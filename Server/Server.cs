using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
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

        private readonly Dictionary<int, ClientHandler> clientHandlersIndexedByUserId = new Dictionary<int, ClientHandler>();
        private readonly EntityIdAllocatorFactory entityIdAllocatorFactory = new EntityIdAllocatorFactory();
        private readonly RepositoryManager repositoryManager = new RepositoryManager();
        private readonly ServerContextRegistry serverContextRegistry;

        public Server()
        {
            serverContextRegistry = new ServerContextRegistry(repositoryManager, clientHandlersIndexedByUserId, entityIdAllocatorFactory);

            repositoryManager.UserRepository.UserAdded += OnUserAdded;
            repositoryManager.UserRepository.UserConnectionUpdated += OnUserConnectionUpdated;
            repositoryManager.UserRepository.UserAvatarUpdated += OnUserAvatarUpdated;
            repositoryManager.ConversationRepository.ContributionAdded += OnContributionAdded;
            repositoryManager.ConversationRepository.ConversationAdded += OnConversationAdded;
            repositoryManager.ParticipationRepository.ParticipationsAdded += OnParticipationsAdded;
            repositoryManager.ParticipationRepository.ParticipationAdded += OnParticipationAdded;

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

                Log.Info("New client connection found. Starting login initialisation process.");

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

            LoginResponse loginResponse = clientHandler.LoginClient(tcpClient, repositoryManager.UserRepository, entityIdAllocatorFactory);

            if (loginResponse.LoginResult == LoginResult.Success)
            {
                int userId = loginResponse.User.UserId;

                clientHandler.CreateConnectionHandler(userId, tcpClient);

                clientHandlersIndexedByUserId[userId] = clientHandler;

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

                IMessageContext context = serverContextRegistry.MessageHandlersIndexedByMessageIdentifier[message.MessageIdentifier];
                handler.HandleMessage(message, context);
            }
            catch (KeyNotFoundException keyNotFoundException)
            {
                Log.Error("ClientService is not supposed to handle message with identifier: " + e.Message.MessageIdentifier,
                    keyNotFoundException);
            }
        }

        private void OnUserAdded(object sender, User user)
        {
            var userNotification = new UserNotification(user, NotificationType.Create);

            foreach (ClientHandler clientHandler in clientHandlersIndexedByUserId.Values)
            {
                clientHandler.SendMessage(userNotification);
            }
        }

        private void OnUserConnectionUpdated(object sender, User user)
        {
            var userNotification = new ConnectionStatusNotification(user.ConnectionStatus, NotificationType.Update);

            foreach (ClientHandler clientHandler in clientHandlersIndexedByUserId.Values)
            {
                clientHandler.SendMessage(userNotification);
            }
        }

        private void OnConversationAdded(object sender, Conversation conversation)
        {
            var conversationNotification = new ConversationNotification(conversation, NotificationType.Create);

            foreach (Participation participant in repositoryManager.ParticipationRepository
                .GetParticipationsByConversationId(
                    conversation.ConversationId))
            {
                clientHandlersIndexedByUserId[participant.UserId].SendMessage(conversationNotification);
            }
        }

        private void OnUserAvatarUpdated(object sender, User user)
        {
            var avatarNotification = new AvatarNotification(user.Avatar, NotificationType.Update);

            foreach (var clientHandler in clientHandlersIndexedByUserId.Values)
            {
                clientHandler.SendMessage(avatarNotification);
            }
        }

        private void OnContributionAdded(object sender, Contribution contribution)
        {
            var contributionNotification = new ContributionNotification(contribution, NotificationType.Create);

            Conversation conversation = repositoryManager.ConversationRepository
                .FindConversationById(contribution.ConversationId);

            foreach (User user in
                repositoryManager.ParticipationRepository.GetParticipationsByConversationId(conversation.ConversationId)
                    .Select(participant => repositoryManager.UserRepository.FindUserById(participant.UserId))
                    .Where(user => user.ConnectionStatus.UserConnectionStatus == ConnectionStatus.Status.Connected))
            {
                clientHandlersIndexedByUserId[user.UserId].SendMessage(contributionNotification);
            }
        }

        private void OnParticipationsAdded(object sender, IEnumerable<Participation> participations)
        {
            IList<Participation> participationsEnumerable = participations as IList<Participation> ?? participations.ToList();

            List<int> userIds = participationsEnumerable.Select(participation => participation.UserId).ToList();

            foreach (Participation participation in participationsEnumerable)
            {
                foreach (int userId in userIds)
                {
                    clientHandlersIndexedByUserId[userId].SendMessage(new ParticipationNotification(participation, NotificationType.Create));
                }
            }
        }

        private void OnParticipationAdded(object sender, Participation participation)
        {
            IEnumerable<Participation> participations =
                repositoryManager.ParticipationRepository.GetParticipationsByConversationId(participation.ConversationId);

            IEnumerable<Participation> conversationParticipations = participations as Participation[] ??
                                                                    participations.ToArray();

            // Update other users of the new conversation participant
            foreach (Participation conversationParticipation in conversationParticipations.Where(
                conversationParticipation => conversationParticipation.UserId != participation.UserId))
            {
                clientHandlersIndexedByUserId[conversationParticipation.UserId].SendMessage(
                    new ParticipationNotification(participation, NotificationType.Create));
            }

            // Give the new user all participations for this new conversation they are entering.
            foreach (Participation conversationParticipation in conversationParticipations)
            {
                clientHandlersIndexedByUserId[participation.UserId].SendMessage(
                    new ParticipationNotification(conversationParticipation, NotificationType.Create));
            }

            Conversation conversation = repositoryManager.ConversationRepository
                .FindConversationById(participation.ConversationId);

            clientHandlersIndexedByUserId[participation.UserId].SendMessage(new ConversationNotification(conversation, NotificationType.Create));

            // Update other conversation participants of conversation update.
            foreach (Participation conversationParticipation in conversationParticipations.Where(conversationParticipation => !conversationParticipation.Equals(participation)))
            {
                clientHandlersIndexedByUserId[conversationParticipation.UserId].SendMessage(new ConversationNotification(conversation, NotificationType.Update));
            }
        }
    }
}