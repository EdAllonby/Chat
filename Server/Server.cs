using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using log4net;
using SharedClasses;
using SharedClasses.Domain;
using SharedClasses.Message;
using SharedClasses.Serialiser;

namespace Server
{
    /// <summary>
    /// Handles the logic for incoming <see cref="IMessage"/> passed from ClientHandler.
    /// </summary>
    internal sealed class Server
    {
        private const int PortNumber = 5004;
        private static readonly ILog Log = LogManager.GetLogger(typeof (Server));

        private readonly IDictionary<int, ClientHandler> clientHandlersIndexedByUserId = new Dictionary<int, ClientHandler>();

        private readonly ConversationIDGenerator conversationIDGenerator = new ConversationIDGenerator();
        private readonly ConversationRepository conversationRepository = new ConversationRepository();
        private readonly UserIDGenerator userIDGenerator = new UserIDGenerator();
        private readonly UserRepository userRepository = new UserRepository();

        public Server()
        {
            Log.Info("Server instance started");
            ListenForNewClients();
        }

        private void ListenForNewClients()
        {
            var clientListener = new TcpListener(IPAddress.Any, PortNumber);
            clientListener.Start();
            Log.Info("Server started listening for clients to connect");

            while (true)
            {
                TcpClient client = clientListener.AcceptTcpClient();
                Log.Info("New client connected");
                client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
                var clientLoginWorkerThread = new Thread(() => InitialiseNewClient(client)) {Name = "Client Login Worker Thread"};
                clientLoginWorkerThread.Start();
            }
        }

        private void InitialiseNewClient(TcpClient tcpClient)
        {
            User newUser = CreateUserEntity(GetClientLoginCredentials(tcpClient));

            NotifyClientsOfUserChange(newUser);

            var loginResponse = new LoginResponse(newUser);

            var clientHandler = new ClientHandler(newUser.UserId, tcpClient);

            clientHandler.OnNewMessage += NewMessageReceived;

            clientHandlersIndexedByUserId[newUser.UserId] = clientHandler;

            clientHandler.SendMessage(loginResponse);
        }

        private static LoginRequest GetClientLoginCredentials(TcpClient tcpClient)
        {
            var messageIdentifierSerialiser = new MessageIdentifierSerialiser();

            int messageIdentifier = messageIdentifierSerialiser.DeserialiseMessageIdentifier(tcpClient.GetStream());

            var serialiserFactory = new SerialiserFactory();

            ISerialiser serialiser = serialiserFactory.GetSerialiser(messageIdentifier);

            var loginRequest = (LoginRequest) serialiser.Deserialise(tcpClient.GetStream());

            return loginRequest;
        }

        private User CreateUserEntity(LoginRequest clientLogin)
        {
            var newUser = new User(clientLogin.User.Username, userIDGenerator.CreateUserId());

            userRepository.AddUser(newUser);

            return newUser;
        }

        private Conversation CreateConversationEntity(ConversationRequest conversationRequest)
        {
            var newConversation = new Conversation(conversationIDGenerator.CreateConversationId(),
                conversationRequest.Conversation.FirstParticipantUserId,
                conversationRequest.Conversation.SecondParticipantUserId);

            conversationRepository.AddConversation(newConversation);

            return newConversation;
        }

        private Contribution CreateContributionEntity(ContributionRequest contributionRequest)
        {
             Conversation conversation = conversationRepository.FindConversationById(contributionRequest.Contribution.ConversationId);

            return conversation.AddContribution(contributionRequest);
        }

        private void NotifyClientsOfUserChange(User user)
        {
            var userNotification = new UserNotification(user, NotificationType.Create);

            foreach (ClientHandler handler in clientHandlersIndexedByUserId.Values)
            {
                handler.SendMessage(userNotification);
            }
        }

        private void NewMessageReceived(object sender, MessageEventArgs e)
        {
            IMessage message = e.Message;

            switch (message.Identifier)
            {
                case MessageNumber.ContributionRequest:
                    Contribution contribution = CreateContributionEntity((ContributionRequest) message);
                    SendContributionNotificationToParticipants(contribution);
                    break;

                case MessageNumber.UserSnapshotRequest:
                    SendUserSnapshot(userRepository.FindUserById(e.ClientUserId));
                    break;

                case MessageNumber.ClientDisconnection:
                    RemoveClientHandler(e.ClientUserId);
                    NotifyClientsOfUserChange(userRepository.FindUserById(e.ClientUserId));
                    userRepository.RemoveUser(e.ClientUserId);
                    break;

                case MessageNumber.ConversationRequest:
                    if (CheckConversationIsValid((ConversationRequest) message))
                    {
                        Conversation conversation = CreateConversationEntity((ConversationRequest) message);
                        SendConversationNotificationToClients(conversation);
                    }
                    break;

                default:
                    Log.Warn("Shared classes assembly does not have a definition for message identifier: " + message.Identifier);
                    break;
            }
        }

        private void SendUserSnapshot(User clientUser)
        {
            IEnumerable<User> currentUsers = userRepository.RetrieveAllUsers();
            var userSnapshot = new UserSnapshot(currentUsers);
            ClientHandler clientHandler = clientHandlersIndexedByUserId[clientUser.UserId];
            clientHandler.SendMessage(userSnapshot);
        }

        private bool CheckConversationIsValid(ConversationRequest conversationRequest)
        {
            if (conversationRequest.Conversation.FirstParticipantUserId == conversationRequest.Conversation.SecondParticipantUserId)
            {
                Log.Warn("Cannot make a conversation between two users of same id of " + conversationRequest.Conversation.FirstParticipantUserId);
                return false;
            }

            if (conversationRepository.GetAllConversations().Any(conversation => (conversationRequest.Conversation.FirstParticipantUserId == conversation.FirstParticipantUserId ||
                                                                                  conversationRequest.Conversation.FirstParticipantUserId == conversation.SecondParticipantUserId) &&
                                                                                 (conversationRequest.Conversation.SecondParticipantUserId == conversation.FirstParticipantUserId ||
                                                                                  conversationRequest.Conversation.SecondParticipantUserId == conversation.SecondParticipantUserId)))
            {
                Log.Warn("Conversation already exists between these two users. Server will not create a new one.");
                return false;
            }

            return true;
        }

        private void SendConversationNotificationToClients(Conversation conversation)
        {
            var conversationNotification = new ConversationNotification(conversation);
            ClientHandler firstParticipantClientHandler = clientHandlersIndexedByUserId[conversation.FirstParticipantUserId];
            firstParticipantClientHandler.SendMessage(conversationNotification);

            ClientHandler secondParticipantClientHandler = clientHandlersIndexedByUserId[conversation.SecondParticipantUserId];
            secondParticipantClientHandler.SendMessage(conversationNotification);
        }

        private void SendContributionNotificationToParticipants(Contribution contribution)
        {
            var contributionNotification = new ContributionNotification(contribution);
            Conversation contributionConversation = conversationRepository.FindConversationById(contribution.ConversationId);

            ClientHandler firstParticipantClientHandler = clientHandlersIndexedByUserId[contributionConversation.FirstParticipantUserId];
            firstParticipantClientHandler.SendMessage(contributionNotification);

            ClientHandler secondParticipantClientHandler = clientHandlersIndexedByUserId[contributionConversation.SecondParticipantUserId];
            secondParticipantClientHandler.SendMessage(contributionNotification);
        }

        private void RemoveClientHandler(int userId)
        {
            clientHandlersIndexedByUserId.Remove(userId);
            Log.Info("User with id " + userId + " logged out. Removing from Server's ClientHandler list");
        }
    }
}