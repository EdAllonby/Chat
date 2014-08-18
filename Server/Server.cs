using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using log4net;
using Server.MessageHandler;
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

        private readonly ClientManager clientManager = new ClientManager();
        private readonly EntityIdAllocatorFactory entityIdAllocatorFactory = new EntityIdAllocatorFactory();
        private readonly RepositoryManager repositoryManager = new RepositoryManager();

        public Server()
        {
            repositoryManager.UserRepository.EntityAdded += OnUserAdded;
            repositoryManager.UserRepository.EntityUpdated += OnUserUpdated;

            repositoryManager.ConversationRepository.EntityAdded += OnConversationAdded;
            repositoryManager.ConversationRepository.EntityUpdated += OnConversationUpdated;

            repositoryManager.ParticipationRepository.EntityAdded += OnParticipationAdded;

            Log.Info("Server instance started");
            ListenForNewClients();
        }

        private void OnParticipationAdded(object sender, EntityChangedEventArgs<Participation> e)
        {
            Participation participation = e.Entity;
            
            List<Participation> conversationParticipants = repositoryManager.ParticipationRepository.GetParticipationsByConversationId(participation.ConversationId);

            ParticipationNotification participationNotification = new ParticipationNotification(participation, NotificationType.Create);

            IEnumerable<int> conversationParticipantUserIds = conversationParticipants.Select(conversationParticipation => conversationParticipation.UserId);

            clientManager.SendMessageToClients(participationNotification, conversationParticipantUserIds);

            List<Participation> otherParticipants = conversationParticipants.Where(conversationParticipant => !conversationParticipant.Equals(participation)).ToList();

            otherParticipants.ForEach(otherParticipant => clientManager.SendMessageToClient(new ParticipationNotification(otherParticipant, NotificationType.Create), participation.UserId));

            Conversation conversation = repositoryManager.ConversationRepository.FindEntityById(participation.ConversationId);

            SendConversationNotificationToParticipants(conversation, participation.UserId, otherParticipants);
        }

        private void SendConversationNotificationToParticipants(Conversation conversation, int newParticipantUserId, IEnumerable<Participation> otherParticipants)
        {
            if (conversation != null)
            {
                clientManager.SendMessageToClient(new ConversationNotification(conversation, NotificationType.Create), newParticipantUserId);

                IEnumerable<int> currentConversationParticipantUserIds = otherParticipants.Select(participant => participant.UserId);

                clientManager.SendMessageToClients(new ConversationNotification(conversation, NotificationType.Update), currentConversationParticipantUserIds);
            }
        }

        private void ListenForNewClients()
        {
            var clientListener = new TcpListener(IPAddress.Any, PortNumber);
            clientListener.Start();
            Log.Info("Server started listening for clients to connect");

            while (true)
            {
                TcpClient client = clientListener.AcceptTcpClient();

                Log.Info("New client connection found. Starting login initialisation process.");

                client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);

                InitialiseNewClient(client);
            }
        }

        private void InitialiseNewClient(TcpClient tcpClient)
        {
            var clientHandler = new ClientHandler();

            LoginResponse loginResponse = clientHandler.InitialiseClient(tcpClient, repositoryManager.UserRepository, entityIdAllocatorFactory);

            if (loginResponse.LoginResult == LoginResult.Success)
            {
                clientManager.AddClientHandler(loginResponse.User.Id, clientHandler);

                clientHandler.MessageReceived += OnMessageReceived;

                Log.InfoFormat("Client with User Id {0} has successfully logged in.", loginResponse.User.Id);
            }
        }

        private void OnMessageReceived(object sender, MessageEventArgs e)
        {
            IMessage message = e.Message;

            try
            {
                IMessageHandler handler =
                    MessageHandlerRegistry.MessageHandlersIndexedByMessageIdentifier[message.MessageIdentifier];

                IServerMessageContext context = new ServerMessageContext(clientManager, entityIdAllocatorFactory, repositoryManager);

                handler.HandleMessage(message, context);
            }
            catch (KeyNotFoundException keyNotFoundException)
            {
                Log.Error("Server is not supposed to handle message with identifier: " + e.Message.MessageIdentifier,
                    keyNotFoundException);
            }
        }

        private void OnUserAdded(object sender, EntityChangedEventArgs<User> e)
        {
            var userNotification = new UserNotification(e.Entity, NotificationType.Create);

            clientManager.SendMessageToClients(userNotification);
        }

        private void OnUserUpdated(object sender, EntityChangedEventArgs<User> e)
        {
            if (e.PreviousEntity.ConnectionStatus.UserConnectionStatus != e.Entity.ConnectionStatus.UserConnectionStatus)
            {
                OnUserConnectionUpdated(e.Entity);
            }
            if (!e.PreviousEntity.Avatar.Equals(e.Entity.Avatar))
            {
                OnUserAvatarUpdated(e.Entity);
            }
        }

        private void OnUserConnectionUpdated(User user)
        {
            var userNotification = new ConnectionStatusNotification(user.ConnectionStatus, NotificationType.Update);

            clientManager.SendMessageToClients(userNotification);
        }

        private void OnUserAvatarUpdated(User user)
        {
            var avatarNotification = new AvatarNotification(user.Avatar, NotificationType.Update);

            clientManager.SendMessageToClients(avatarNotification);
        }

        private void OnConversationAdded(Conversation conversation)
        {
            var conversationNotification = new ConversationNotification(conversation, NotificationType.Create);

            IEnumerable<int> userIds = repositoryManager.ParticipationRepository.GetParticipationsByConversationId(conversation.Id).Select(participation => participation.UserId);

            clientManager.SendMessageToClients(conversationNotification, userIds);
        }

        private void OnConversationAdded(object sender, EntityChangedEventArgs<Conversation> e)
        {
            OnConversationAdded(e.Entity);
        }

        private void OnConversationUpdated(object sender, EntityChangedEventArgs<Conversation> e)
        {
            if (!e.Entity.LastContribution.Equals(e.PreviousEntity.LastContribution))
            {
                OnContributionAdded(e.Entity.LastContribution);
            }
        }

        private void OnContributionAdded(Contribution contribution)
        {
            var contributionNotification = new ContributionNotification(contribution, NotificationType.Create);

            List<Participation> participationsByConversationId = repositoryManager.ParticipationRepository.GetParticipationsByConversationId(contribution.ConversationId);
            IEnumerable<User> participantUsers = participationsByConversationId.Select(participant => repositoryManager.UserRepository.FindEntityById(participant.UserId));
            IEnumerable<int> connectedUserIds = participantUsers.Where(user => user.ConnectionStatus.UserConnectionStatus == ConnectionStatus.Status.Connected).Select(user => user.Id);

            clientManager.SendMessageToClients(contributionNotification, connectedUserIds);
        }
    }
}