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
    /// Handles the logic for incoming <see cref="IMessage"/> passed from ConnectionHandler.
    /// </summary>
    internal sealed class Server
    {
        private const int PortNumber = 5004;
        private static readonly ILog Log = LogManager.GetLogger(typeof (Server));

        private readonly IDictionary<int, ConnectionHandler> clientConnectionHandlersIndexedByUserId = new Dictionary<int, ConnectionHandler>();

        private readonly RepositoryFactory repositoryFactory = new RepositoryFactory();

        private readonly EntityIDGenerator userIDGenerator = new EntityIDGenerator();
        private readonly EntityIDGenerator conversationIDGenerator = new EntityIDGenerator();
        private readonly EntityIDGenerator contributionIDGenerator = new EntityIDGenerator();


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

            var connectionHandler = new ConnectionHandler(newUser.UserId, tcpClient);

            connectionHandler.OnNewMessage += NewMessageReceived;

            clientConnectionHandlersIndexedByUserId[newUser.UserId] = connectionHandler;

            connectionHandler.SendMessage(loginResponse);
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
            var newUser = new User(clientLogin.User.Username, userIDGenerator.AssignEntityID());

            repositoryFactory.GetRepository<User>().AddEntity(newUser);

            return newUser;
        }

        private Conversation CreateConversationEntity(ConversationRequest conversationRequest)
        {
            var newConversation = new Conversation(conversationIDGenerator.AssignEntityID(),
                conversationRequest.Conversation.FirstParticipantUserId,
                conversationRequest.Conversation.SecondParticipantUserId);

            repositoryFactory.GetRepository<Conversation>().AddEntity(newConversation);

            return newConversation;
        }

        private Contribution CreateContributionEntity(ContributionRequest contributionRequest)
        {
            var newContribution = new Contribution(contributionIDGenerator.AssignEntityID(), contributionRequest.Contribution);

            Conversation conversation = repositoryFactory.GetRepository<Conversation>().FindEntityByID(newContribution.ConversationId);

            conversation.AddContribution(newContribution);

            return newContribution;
        }

        private void NotifyClientsOfUserChange(User user)
        {
            var userNotification = new UserNotification(user, NotificationType.Create);

            foreach (ConnectionHandler handler in clientConnectionHandlersIndexedByUserId.Values)
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
                    SendUserSnapshot(repositoryFactory.GetRepository<User>().FindEntityByID(e.ClientUserId));
                    break;

                case MessageNumber.ClientDisconnection:
                    RemoveClientHandler(e.ClientUserId);

                    IEntityRepository<User> userRepository = repositoryFactory.GetRepository<User>();
                    NotifyClientsOfUserChange(userRepository.FindEntityByID(e.ClientUserId));
                    userRepository.RemoveEntity(e.ClientUserId);
                    break;

                case MessageNumber.ConversationRequest:
                    if (CheckConversationIsValid((ConversationRequest) message))
                    {
                        Conversation conversation = CreateConversationEntity((ConversationRequest) message);
                        SendConversationNotificationToClients(conversation);
                    }
                    break;

                default:
                    Log.Warn("Server is not supposed to handle message with identifier: " + message.Identifier);
                    break;
            }
        }

        private void SendUserSnapshot(User clientUser)
        {
            IEnumerable<User> currentUsers = repositoryFactory.GetRepository<User>().GetAllEntities();
            var userSnapshot = new UserSnapshot(currentUsers);
            ConnectionHandler connectionHandler = clientConnectionHandlersIndexedByUserId[clientUser.UserId];
            connectionHandler.SendMessage(userSnapshot);
        }

        private bool CheckConversationIsValid(ConversationRequest conversationRequest)
        {
            if (conversationRequest.Conversation.FirstParticipantUserId == conversationRequest.Conversation.SecondParticipantUserId)
            {
                Log.Warn("Cannot make a conversation between two users of same id of " + conversationRequest.Conversation.FirstParticipantUserId);
                return false;
            }

            IEnumerable<Conversation> conversations = repositoryFactory.GetRepository<Conversation>().GetAllEntities();
            if (conversations.Any(conversation => (conversationRequest.Conversation.FirstParticipantUserId == conversation.FirstParticipantUserId ||
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
            ConnectionHandler firstParticipantConnectionHandler = clientConnectionHandlersIndexedByUserId[conversation.FirstParticipantUserId];
            firstParticipantConnectionHandler.SendMessage(conversationNotification);

            ConnectionHandler secondParticipantConnectionHandler = clientConnectionHandlersIndexedByUserId[conversation.SecondParticipantUserId];
            secondParticipantConnectionHandler.SendMessage(conversationNotification);
        }

        private void SendContributionNotificationToParticipants(Contribution contribution)
        {
            var contributionNotification = new ContributionNotification(contribution);
            Conversation contributionConversation = repositoryFactory.GetRepository<Conversation>().FindEntityByID(contribution.ConversationId);

            ConnectionHandler firstParticipantConnectionHandler = clientConnectionHandlersIndexedByUserId[contributionConversation.FirstParticipantUserId];
            firstParticipantConnectionHandler.SendMessage(contributionNotification);

            ConnectionHandler secondParticipantConnectionHandler = clientConnectionHandlersIndexedByUserId[contributionConversation.SecondParticipantUserId];
            secondParticipantConnectionHandler.SendMessage(contributionNotification);
        }

        private void RemoveClientHandler(int userId)
        {
            clientConnectionHandlersIndexedByUserId.Remove(userId);
            Log.Info("User with id " + userId + " logged out. Removing from Server's ConnectionHandler list");
        }
    }
}