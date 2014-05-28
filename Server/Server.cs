using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using log4net;
using SharedClasses;
using SharedClasses.Domain;
using SharedClasses.Message;

namespace Server
{
    /// <summary>
    /// Handles the logic for incoming <see cref="IMessage"/> passed from ConnectionHandler.
    /// </summary>
    internal sealed class Server
    {
        private const int PortNumber = 5004;
        private static readonly ILog Log = LogManager.GetLogger(typeof (Server));
        
        private readonly EntityGeneratorFactory entityIDGenerator = new EntityGeneratorFactory();

        private readonly RepositoryManager repositoryManager = new RepositoryManager();

        private readonly Dictionary<int, ClientHandler> clientHandlersIndexedByUserId = new Dictionary<int, ClientHandler>(); 

        public Server()
        {
            repositoryManager.UserRepository.UserAdded += OnNewUser;
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
                var clientLoginWorkerThread = new Thread(() => InitialiseNewClient(client))
                {
                    Name = "Client Login Worker Thread"
                };

                clientLoginWorkerThread.Start();
            }
        }

        private void InitialiseNewClient(TcpClient tcpClient)
        {
            var clientHandler = new ClientHandler();

            LoginResponse loginResponse = clientHandler.LoginClient(tcpClient, entityIDGenerator, repositoryManager);

            if (loginResponse.LoginResult == LoginResult.Success)
            {
                int userId = loginResponse.User.UserId;

                clientHandler.CreateConnectionHandler(userId, tcpClient);

                clientHandlersIndexedByUserId[userId] = clientHandler;
                
                clientHandler.ConnectionHandler.OnNewMessage += NewMessageReceived;
                Log.InfoFormat("Client with User Id {0} has successfully logged in.", userId);
            }
        }

        private int CreateConversationEntity(ConversationRequest conversationRequest)
        {
            int conversationId = entityIDGenerator.GetEntityID<Conversation>();

            var newConversation = new Conversation(conversationId);

            foreach (int participantId in conversationRequest.ParticipantIds)
            {
                repositoryManager.ParticipationRepository.AddParticipation(new Participation(participantId, conversationId));
            }

            repositoryManager.ConversationRepository.AddConversation(newConversation);

            return conversationId;
        }

        private Contribution CreateContributionEntity(ContributionRequest contributionRequest)
        {
            var newContribution = new Contribution(entityIDGenerator.GetEntityID<Contribution>(),
                contributionRequest.Contribution);

            Conversation conversation = repositoryManager.ConversationRepository.FindConversationById(newContribution.ConversationId);

            conversation.AddContribution(newContribution);

            return newContribution;
        }

        private void OnNewUser(User user)
        {
            var userNotification = new UserNotification(user, NotificationType.Update);

            foreach (ClientHandler clientHandler in clientHandlersIndexedByUserId.Values)
            {
                clientHandler.SendMessage(userNotification);
            }
        }

        private void NewMessageReceived(object sender, MessageEventArgs e)
        {
            IMessage message = e.Message;

            switch (message.Identifier)
            {
                case MessageNumber.UserSnapshotRequest:
                    SendUserSnapshot((UserSnapshotRequest)message);
                    break;

                case MessageNumber.ConversationSnapshotRequest:
                    SendConversationSnapshot((ConversationSnapshotRequest)message);
                    break;

                case MessageNumber.ParticipationSnapshotRequest:
                    SendParticipationSnapshot((ParticipationSnapshotRequest) message);
                    break;

                case MessageNumber.ContributionRequest:
                    Contribution contribution = CreateContributionEntity((ContributionRequest) message);
                    SendContributionNotificationToParticipants(contribution);
                    break;

                case MessageNumber.ClientDisconnection:
                    var clientDisconnection = (ClientDisconnection) message;
                    RemoveClientHandler(clientDisconnection.UserId);
                    DisconnectUser(clientDisconnection.UserId);
                    break;

                case MessageNumber.ConversationRequest:
                    if (CheckConversationIsValid((ConversationRequest) message))
                    {
                        var conversationRequest = (ConversationRequest) message;
                        int conversationId = CreateConversationEntity(conversationRequest);
                        SendConversationNotificationToClients(conversationRequest.ParticipantIds, conversationId);
                    }
                    break;

                default:
                    Log.Warn("Server is not supposed to handle message with identifier: " + message.Identifier);
                    break;
            }
        }

        private void SendUserSnapshot(UserSnapshotRequest userSnapshotRequest)
        {
            IEnumerable<User> currentUsers = repositoryManager.UserRepository.GetAllUsers();
            var userSnapshot = new UserSnapshot(currentUsers);
            clientHandlersIndexedByUserId[userSnapshotRequest.UserId].SendMessage(userSnapshot);
        }

        private void SendConversationSnapshot(ConversationSnapshotRequest conversationSnapshotRequest)
        {
            IEnumerable<int> conversationIds = repositoryManager.ParticipationRepository.GetAllConversationIdsByUserId(conversationSnapshotRequest.UserId);

            IList<int> conversationEnumerable = conversationIds as IList<int> ?? conversationIds.ToList();

            List<Conversation> conversations = conversationEnumerable.Select(conversationId => repositoryManager.ConversationRepository.FindConversationById(conversationId)).ToList();

            var conversationSnapshot = new ConversationSnapshot(conversations);

            clientHandlersIndexedByUserId[conversationSnapshotRequest.UserId].SendMessage(conversationSnapshot);
        }

        private void SendParticipationSnapshot(ParticipationSnapshotRequest participationSnapshotRequest)
        {
            var userParticipations = new List<Participation>();

            foreach (int conversationId in repositoryManager.ParticipationRepository.GetAllConversationIdsByUserId(participationSnapshotRequest.UserId))
            {
                userParticipations.AddRange(repositoryManager.ParticipationRepository.GetParticipationsByConversationId(conversationId));
            }

            var participationSnapshot = new ParticipationSnapshot(userParticipations);

            clientHandlersIndexedByUserId[participationSnapshotRequest.UserId].SendMessage(participationSnapshot);
        }

        private void DisconnectUser(int userId)
        {
            User user = repositoryManager.UserRepository.FindUserByID(userId);
            user.ConnectionStatus = ConnectionStatus.Disconnected;
            repositoryManager.UserRepository.AddUser(user);
        }

        private bool CheckConversationIsValid(ConversationRequest conversationRequest)
        {
            // Check for no repeating users
            if (conversationRequest.ParticipantIds.Count != conversationRequest.ParticipantIds.Distinct().Count())
            {
                Log.Warn("Cannot make a conversation between two users of same id");
                return false;
            }

            return !repositoryManager.ParticipationRepository.DoesConversationWithUsersExist(conversationRequest.ParticipantIds);
        }

        private void SendConversationNotificationToClients(List<int> participantIds, int conversationId)
        {
            var conversationNotification = new ConversationNotification(participantIds, conversationId);

            foreach (var participantId in participantIds.Where(participantId => clientHandlersIndexedByUserId.ContainsKey(participantId)))
            {
                clientHandlersIndexedByUserId[participantId].SendMessage(conversationNotification);
            }
        }

        private void SendContributionNotificationToParticipants(Contribution contribution)
        {
            var contributionNotification = new ContributionNotification(contribution);
            Conversation conversation = repositoryManager.ConversationRepository.FindConversationById(contribution.ConversationId);

            foreach (User user in repositoryManager.ParticipationRepository.GetParticipationsByConversationId(conversation.ConversationId)
                .Select(participant => repositoryManager.UserRepository.FindUserByID(participant.UserId))
                .Where(user => user.ConnectionStatus == ConnectionStatus.Connected))
            {
                clientHandlersIndexedByUserId[user.UserId].SendMessage(contributionNotification);
            }
        }

        private void RemoveClientHandler(int userId)
        {
            clientHandlersIndexedByUserId.Remove(userId);
            Log.Info("User with id " + userId + " logged out. Removing from ServerHandler's ConnectionHandler list");
        }
    }
}