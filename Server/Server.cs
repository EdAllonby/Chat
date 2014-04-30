using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using log4net;
using SharedClasses;
using SharedClasses.Domain;
using SharedClasses.Protocol;

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

        private readonly ContributionIDGenerator contributionIDGenerator = new ContributionIDGenerator();
        private readonly ContributionRepository contributionRepository = new ContributionRepository();
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
                var clientLoginWorkerThread = new Thread(() => InitialiseNewClient(client)) { Name = "Client Login Worker Thread"};
                clientLoginWorkerThread.Start();
            }
        }

        private void InitialiseNewClient(TcpClient tcpClient)
        {
            User newUser = CreateUserEntity(GetClientLoginCredentials(tcpClient));

            var clientHandler = new ClientHandler(newUser.UserId, tcpClient);

            NotifyClientsOfNewUser(newUser);

            clientHandlersIndexedByUserId[newUser.UserId] = clientHandler;

            var userNotification = new UserNotification(newUser, NotificationType.Create);

            clientHandler.SendMessage(userNotification);

            clientHandler.OnNewMessage += NewMessageReceived;

            clientHandler.CreateListenerThreadForClient();
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
            var newContribution = new Contribution(
                contributionIDGenerator.CreateConversationId(),
                contributionRequest.Contribution);

            contributionRepository.AddContribution(newContribution);

            return newContribution;
        }

        private void NotifyClientsOfNewUser(User user)
        {
            foreach (ClientHandler handler in clientHandlersIndexedByUserId.Values)
            {
                var userNotification = new UserNotification(user, NotificationType.Create);
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
                    SendUserSnapshot(userRepository.FindUserByID(e.ClientUserId));
                    break;

                case MessageNumber.ClientDisconnection:
                    RemoveClientHandler(userRepository.FindUserByID(e.ClientUserId));
                    NotifyClientsOfDisconnectedUser(userRepository.FindUserByID(e.ClientUserId));
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
            IEnumerable<User> currentUsers = userRepository.AllUsers();
            var userSnapshot = new UserSnapshot(currentUsers);
            ClientHandler clientHandler = clientHandlersIndexedByUserId[clientUser.UserId];
            clientHandler.SendMessage(userSnapshot);
        }

        private static bool CheckConversationIsValid(ConversationRequest conversationRequest)
        {
            if (conversationRequest.Conversation.FirstParticipantUserId == conversationRequest.Conversation.SecondParticipantUserId)
            {
                Log.Warn("Cannot make a conversation between two users of same id of " + conversationRequest.Conversation.FirstParticipantUserId);
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

        private void RemoveClientHandler(User clientUser)
        {
            clientHandlersIndexedByUserId.Remove(clientUser.UserId);

            Log.Info("User with id " + clientUser.UserId + " logged out. Removing from Server's ClientHandler list");

        }

        private void NotifyClientsOfDisconnectedUser(User disconnectedUser)
        {
            var userNotification = new UserNotification(disconnectedUser, NotificationType.Delete);

            Parallel.ForEach(clientHandlersIndexedByUserId.Values, clientHandler => clientHandler.SendMessage(userNotification));
        }
    }
}