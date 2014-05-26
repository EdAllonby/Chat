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

        private readonly ClientLoginHandler clientLoginHandler;

        private readonly ClientHandler clientHandler = new ClientHandler();
        
        private readonly EntityGeneratorFactory entityIDGenerator = new EntityGeneratorFactory();

        private readonly UserRepository userRepository = new UserRepository();
        private readonly ParticipationRepository participationRepository = new ParticipationRepository();
        private readonly ConversationRepository conversationRepository = new ConversationRepository();

        public Server()
        {
            Log.Info("Server instance started");
            clientLoginHandler = new ClientLoginHandler(clientHandler, entityIDGenerator, userRepository, conversationRepository, participationRepository);
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
            LoginResponse loginResponse = clientLoginHandler.InitialiseNewClient(tcpClient);

            if (loginResponse.LoginResult == LoginResult.Success)
            {
                clientHandler.AddNewConnectionHandler(loginResponse.User.UserId, tcpClient);
                clientHandler.GetConnectionHandler(loginResponse.User.UserId).OnNewMessage += NewMessageReceived;
            }
        }

        private int CreateConversationEntity(ConversationRequest conversationRequest)
        {
            int conversationId = entityIDGenerator.GetEntityID<Conversation>();

            var newConversation = new Conversation(conversationId);

            foreach (int participantId in conversationRequest.ParticipantIds)
            {
                participationRepository.AddParticipation(new Participation(participantId, conversationId));
            }

            conversationRepository.AddConversation(newConversation);

            return conversationId;
        }

        private Contribution CreateContributionEntity(ContributionRequest contributionRequest)
        {
            var newContribution = new Contribution(entityIDGenerator.GetEntityID<Contribution>(),
                contributionRequest.Contribution);

            Conversation conversation = conversationRepository.FindConversationById(newContribution.ConversationId);

            conversation.AddContribution(newContribution);

            return newContribution;
        }

        private void NotifyClientsOfUser(User user, NotificationType updateType, ConnectionStatus status)
        {
            user.ConnectionStatus = status;

            var userNotification = new UserNotification(user, updateType);

            clientHandler.NotifyAllClients(userNotification);
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

                case MessageNumber.ClientDisconnection:
                    var clientDisconnection = (ClientDisconnection) message;
                    RemoveClientHandler(clientDisconnection.UserId);
                    NotifyClientsOfUser(userRepository.FindUserByID(clientDisconnection.UserId), NotificationType.Update,
                        ConnectionStatus.Disconnected);
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

        private bool CheckConversationIsValid(ConversationRequest conversationRequest)
        {
            // Check for no repeating users
            if (conversationRequest.ParticipantIds.Count != conversationRequest.ParticipantIds.Distinct().Count())
            {
                Log.Warn("Cannot make a conversation between two users of same id");
                return false;
            }

            return !participationRepository.DoesConversationWithUsersExist(conversationRequest.ParticipantIds);
        }

        private void SendConversationNotificationToClients(List<int> participantIds, int conversationId)
        {
            var conversationNotification = new ConversationNotification(participantIds, conversationId);

            // Send message to each user in conversation
            foreach (ConnectionHandler participantConnectionHandler in participantIds
                .Select(participant => clientHandler.GetConnectionHandler(participant)))
            {
                participantConnectionHandler.SendMessage(conversationNotification);
            }
        }

        private void SendContributionNotificationToParticipants(Contribution contribution)
        {
            var contributionNotification = new ContributionNotification(contribution);
            Conversation conversation = conversationRepository.FindConversationById(contribution.ConversationId);

            foreach (User user in participationRepository.GetParticipationsByConversationId(conversation.ConversationId)
                .Select(participant => userRepository.FindUserByID(participant.UserId))
                .Where(user => user.ConnectionStatus == ConnectionStatus.Connected))
            {
                clientHandler.GetConnectionHandler(user.UserId).SendMessage(contributionNotification);
            }
        }

        private void RemoveClientHandler(int userId)
        {
            clientHandler.RemoveConnectionHander(userId);
            Log.Info("User with id " + userId + " logged out. Removing from ServerHandler's ConnectionHandler list");
        }
    }
}