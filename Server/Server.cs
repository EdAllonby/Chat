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
            repositoryManager.UserRepository.EntityChanged += OnUserChanged;

            repositoryManager.ConversationRepository.EntityChanged += OnConversationChanged;
            repositoryManager.ParticipationRepository.EntitiesAdded += OnParticipationsAdded;
            repositoryManager.ParticipationRepository.EntityChanged += ParticipationEntityChanged;

            Log.Info("Server instance started");
            ListenForNewClients();
        }

        private void ParticipationEntityChanged(object sender, EntityChangedEventArgs<Participation> e)
        {
            if (e.NotificationType == NotificationType.Create)
            {
                OnParticipationAdded(e.Entity);
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

            LoginResponse loginResponse = clientHandler.LoginClient(tcpClient, repositoryManager.UserRepository, entityIdAllocatorFactory);

            if (loginResponse.LoginResult == LoginResult.Success)
            {
                int userId = loginResponse.User.Id;

                clientHandler.CreateConnectionHandler(userId, tcpClient);

                clientManager.AddClientHandler(userId, clientHandler);

                clientHandler.MessageReceived += OnMessageReceived;

                Log.InfoFormat("Client with User Id {0} has successfully logged in.", userId);
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


        private void OnUserChanged(object sender, EntityChangedEventArgs<User> e)
        {
            switch (e.NotificationType)
            {
                case NotificationType.Create:
                    OnUserAdded(e.Entity);
                    break;
                case NotificationType.Update:
                    if (e.PreviousEntity.ConnectionStatus.UserConnectionStatus != e.Entity.ConnectionStatus.UserConnectionStatus)
                    {
                        OnUserConnectionUpdated(e.Entity);
                    }
                    if (!e.PreviousEntity.Avatar.Equals(e.Entity.Avatar))
                    {
                        OnUserAvatarUpdated(e.Entity);
                    }
                    break;
            }
        }

        private void OnUserAdded(User user)
        {
            var userNotification = new UserNotification(user, NotificationType.Create);

            clientManager.SendMessageToClients(userNotification);
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

        private void OnConversationChanged(object sender, EntityChangedEventArgs<Conversation> e)
        {
            switch (e.NotificationType)
            {
                case NotificationType.Create:
                    OnConversationAdded(e.Entity);
                    break;
                case NotificationType.Update:
                    if (!e.Entity.LastContribution.Equals(e.PreviousEntity.LastContribution))
                    {
                        OnContributionAdded(e.Entity.LastContribution);
                    }
                    break;
            }
        }

        private void OnContributionAdded(Contribution contribution)
        {
            var contributionNotification = new ContributionNotification(contribution, NotificationType.Create);

            IEnumerable<int> userIds = repositoryManager.ParticipationRepository.GetParticipationsByConversationId(contribution.ConversationId)
                .Select(participant => repositoryManager.UserRepository.FindEntityById(participant.UserId))
                .Where(user => user.ConnectionStatus.UserConnectionStatus == ConnectionStatus.Status.Connected).Select(user => user.Id);

            clientManager.SendMessageToClients(contributionNotification, userIds);
        }

        private void OnParticipationsAdded(object sender, IEnumerable<Participation> participations)
        {
            IList<Participation> participationsEnumerable = participations as IList<Participation> ?? participations.ToList();

            List<int> userIds = participationsEnumerable.Select(participation => participation.UserId).ToList();

            foreach (Participation participation in participationsEnumerable)
            {
                var participationNotification = new ParticipationNotification(participation, NotificationType.Create);
                clientManager.SendMessageToClients(participationNotification, userIds);
            }
        }

        private void OnParticipationAdded(Participation participation)
        {
            IEnumerable<Participation> participations =
                repositoryManager.ParticipationRepository.GetParticipationsByConversationId(participation.ConversationId);

            IEnumerable<Participation> conversationParticipations = participations as Participation[] ??
                                                                    participations.ToArray();

            // Update other users of the new conversation participant
            IEnumerable<int> userIds = conversationParticipations.Where(conversationParticipation => conversationParticipation.UserId != participation.UserId).Select(participant => participant.UserId);

            var newParticipantNotification = new ParticipationNotification(participation, NotificationType.Create);

            clientManager.SendMessageToClients(newParticipantNotification, userIds);

            // Give the new user all participations for this new conversation they are entering.
            foreach (Participation conversationParticipation in conversationParticipations)
            {
                clientManager.SendMessageToClient(new ParticipationNotification(conversationParticipation, NotificationType.Create), participation.UserId);
            }

            Conversation conversation = repositoryManager.ConversationRepository.FindEntityById(participation.ConversationId);

            clientManager.SendMessageToClient(new ConversationNotification(conversation, NotificationType.Create), participation.UserId);

            // Update other conversation participants of conversation update.
            IEnumerable<int> currentConversationParticipantsUserIds = conversationParticipations.Where(conversationParticipation => !conversationParticipation.Equals(participation)).Select(participant => participant.UserId);

            clientManager.SendMessageToClients(new ConversationNotification(conversation, NotificationType.Update), currentConversationParticipantsUserIds);
        }
    }
}