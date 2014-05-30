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
        
        private readonly RepositoryManager repositoryManager = new RepositoryManager();

        private readonly Dictionary<int, ClientHandler> clientHandlersIndexedByUserId = new Dictionary<int, ClientHandler>(); 

        public Server()
        {
            repositoryManager.UserRepository.UserUpdated += OnUserUpdated;
            repositoryManager.ConversationRepository.ContributionAdded += OnContributionAdded;
            repositoryManager.ConversationRepository.ConversationAdded += OnConversationAdded;
            repositoryManager.ParticipationRepository.ParticipationsAdded += OnParticipationsAdded;

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

            LoginResponse loginResponse = clientHandler.LoginClient(tcpClient, repositoryManager);

            if (loginResponse.LoginResult == LoginResult.Success)
            {
                int userId = loginResponse.User.UserId;

                clientHandler.CreateConnectionHandler(userId, tcpClient);

                clientHandlersIndexedByUserId[userId] = clientHandler;
                
                clientHandler.MessageReceived += OnMessageReceived;
                Log.InfoFormat("Client with User Id {0} has successfully logged in.", userId);
            }
        }

        private void CreateConversationEntity(NewConversationRequest newConversationRequest)
        {
            int conversationId = EntityGeneratorFactory.GetEntityID<Conversation>();

            var newConversation = new Conversation(conversationId);

            IEnumerable<Participation> participations = newConversationRequest.ParticipantIds
                .Select(participantId => new Participation(participantId, conversationId));

            repositoryManager.ParticipationRepository.AddParticipations(participations);

            repositoryManager.ConversationRepository.AddConversation(newConversation);
        }

        private void CreateContributionEntity(ContributionRequest contributionRequest)
        {
            var newContribution = new Contribution(EntityGeneratorFactory.GetEntityID<Contribution>(),
                contributionRequest.Contribution);
            repositoryManager.ConversationRepository.AddContributionToConversation(newContribution);
        }

        private void OnUserUpdated(User user)
        {
            var userNotification = new UserNotification(user, NotificationType.Update);

            foreach (ClientHandler clientHandler in clientHandlersIndexedByUserId.Values)
            {
                clientHandler.SendMessage(userNotification);
            }
        }

        private void OnMessageReceived(object sender, MessageEventArgs e)
        {
            IMessage message = e.Message;

            switch (message.MessageIdentifier)
            {
                case MessageIdentifier.UserSnapshotRequest:
                    SendUserSnapshot((UserSnapshotRequest)message);
                    break;

                case MessageIdentifier.ConversationSnapshotRequest:
                    SendConversationSnapshot((ConversationSnapshotRequest)message);
                    break;

                case MessageIdentifier.ParticipationSnapshotRequest:
                    SendParticipationSnapshot((ParticipationSnapshotRequest) message);
                    break;

                case MessageIdentifier.ContributionRequest:
                    CreateContributionEntity((ContributionRequest) message);
                    break;

                case MessageIdentifier.ClientDisconnection:
                    var clientDisconnection = (ClientDisconnection) message;
                    RemoveClientHandler(clientDisconnection.UserId);
                    DisconnectUser(clientDisconnection.UserId);
                    break;

                case MessageIdentifier.NewConversationRequest:
                    if (CheckConversationIsValid((NewConversationRequest) message))
                    {
                        var conversationRequest = (NewConversationRequest) message;
                        CreateConversationEntity(conversationRequest);
                    }
                    break;

                default:
                    Log.Warn("Server is not supposed to handle message with identifier: " + message.MessageIdentifier);
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
            repositoryManager.UserRepository.UpdateUser(user);
        }

        private bool CheckConversationIsValid(NewConversationRequest newConversationRequest)
        {
            // Check for no repeating users
            if (newConversationRequest.ParticipantIds.Count != newConversationRequest.ParticipantIds.Distinct().Count())
            {
                Log.Warn("Cannot make a conversation between two users of same id");
                return false;
            }

            return !repositoryManager.ParticipationRepository.DoesConversationWithUsersExist(newConversationRequest.ParticipantIds);
        }

        private void OnConversationAdded(Conversation conversation)
        {
            var conversationNotification = new ConversationNotification(conversation);
            foreach (Participation participant in repositoryManager.ParticipationRepository.GetParticipationsByConversationId(conversation.ConversationId))
            {
                clientHandlersIndexedByUserId[participant.UserId].SendMessage(conversationNotification);
            }
        }

        private void OnParticipationsAdded(IEnumerable<Participation> participations)
        {
            IEnumerable<Participation> participationsEnumerable = participations as IList<Participation> ?? participations.ToList();
            
            List<int> userIds = participationsEnumerable.Select(participation => participation.UserId).ToList();

            var participationsNotification = new ParticipationsNotification(userIds, participationsEnumerable.First().ConversationId);
            foreach (int userId in userIds)
            {
                clientHandlersIndexedByUserId[userId].SendMessage(participationsNotification);
            }
        }

        private void OnContributionAdded(Contribution contribution)
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