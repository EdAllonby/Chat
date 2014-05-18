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
      
        private readonly List<Participation> participations = new List<Participation>(); 

        private readonly EntityIDGenerator contributionIDGenerator = new EntityIDGenerator();
        private readonly EntityIDGenerator conversationIDGenerator = new EntityIDGenerator();

        private readonly RepositoryFactory repositoryFactory = new RepositoryFactory();

        private readonly EntityIDGenerator userIDGenerator = new EntityIDGenerator();


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

            MessageNumber messageIdentifier = messageIdentifierSerialiser.DeserialiseMessageIdentifier(tcpClient.GetStream());

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

        private int CreateConversationEntity(ConversationRequest conversationRequest)
        {
            int conversationId = conversationIDGenerator.AssignEntityID();

            var newConversation = new Conversation(conversationId);

            foreach (int participantId in conversationRequest.ParticipantIds)
            {
                participations.Add(new Participation(participantId, conversationId));
            }

            repositoryFactory.GetRepository<Conversation>().AddEntity(newConversation);

            return conversationId;
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
                        var conversationRequest = (ConversationRequest) message;
                        int conversationId = CreateConversationEntity(conversationRequest);
                        SendConversationNotificationToClients(conversationRequest.ParticipantIds,  conversationId);
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
            // Check for no repeating users
            if (conversationRequest.ParticipantIds.Count != conversationRequest.ParticipantIds.Distinct().Count())
            {
                Log.Warn("Cannot make a conversation between two users of same id");
                return false;
            }

            // if conversation already exists
            var userIdsIndexedByConversationId = new Dictionary<int, List<int>>();

            foreach (var participation in participations)
            {
                if (!userIdsIndexedByConversationId.ContainsKey(participation.ConversationId))
                {
                    userIdsIndexedByConversationId[participation.ConversationId] = new List<int>();
                }

                userIdsIndexedByConversationId[participation.ConversationId].Add(participation.UserId);
            }

            if ((from conversationKeyValuePair
                in userIdsIndexedByConversationId
                let isConversation = conversationKeyValuePair.Value.HasSameElementsAs(conversationRequest.ParticipantIds)
                where isConversation
                select conversationKeyValuePair).Any())
            {
                Log.Warn("Conversation already exists between these two users. Server will not create a new one.");
                return false;
            }

            return true;
        }

        private void SendConversationNotificationToClients(List<int> participantIds, int conversationId)
        {
            var conversationNotification = new ConversationNotification(participantIds, conversationId);

            // Send message to each user in conversation
            foreach (ConnectionHandler participantConnectionHandler in participantIds
                .Select(participant => clientConnectionHandlersIndexedByUserId[participant]))
            {
                participantConnectionHandler.SendMessage(conversationNotification);
            }
        }

        private void SendContributionNotificationToParticipants(Contribution contribution)
        {
            var contributionNotification = new ContributionNotification(contribution);
            Conversation conversation = repositoryFactory.GetRepository<Conversation>().FindEntityByID(contribution.ConversationId);

            // Send message to each user in conversation
            foreach (var participant in participations.Where(x => x.ConversationId == conversation.ConversationId))
            {
                ConnectionHandler participantConnectionHandler = clientConnectionHandlersIndexedByUserId[participant.UserId];
                participantConnectionHandler.SendMessage(contributionNotification);
            }
        }

        private void RemoveClientHandler(int userId)
        {
            clientConnectionHandlersIndexedByUserId.Remove(userId);
            Log.Info("User with id " + userId + " logged out. Removing from Server's ConnectionHandler list");
        }
    }
}