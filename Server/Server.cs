using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
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
        private readonly IList<ClientHandler> clientHandlers = new List<ClientHandler>();

        private readonly TcpListener clientListener = new TcpListener(IPAddress.Any, PortNumber);

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
            clientListener.Start();
            Log.Info("Server started listening for clients to connect");

            while (true)
            {
                TcpClient client = clientListener.AcceptTcpClient();
                Log.Info("New client connected");
                client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
                var newClientThread = new Thread(() => ClientThread(client));
                newClientThread.Start();
            }
        }

        private void ClientThread(TcpClient client)
        {
            var clientHandler = new ClientHandler(client);

            User user = CreateUserEntity(clientHandler.GetClientLoginCredentials());

            clientHandler.AddUserToClientHandler(user);

            NotifyClientsOfNewUser(user);

            clientHandlers.Add(clientHandler);

            var loginResponse = new LoginResponse(user);

            clientHandler.SendMessage(loginResponse);

            clientHandler.OnNewMessage += NewMessageReceived;

            clientHandler.CreateListenerThreadForClient(client);
        }

        private User CreateUserEntity(LoginRequest clientLogin)
        {
            var user = new User(clientLogin.UserName, userIDGenerator.CreateUserId());
            userRepository.AddUser(user);
            return user;
        }

        private void NotifyClientsOfNewUser(User user)
        {
            foreach (ClientHandler handler in clientHandlers)
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
                    AddContributionToRepository(contribution);
                    SendContributionNotificationToParticipants(contribution);
                    break;

                case MessageNumber.UserSnapshotRequest:
                    SendUserSnapshot(e.ClientUser);
                    break;

                case MessageNumber.ClientDisconnection:
                    RemoveClientHandler(e.ClientUser);
                    NotifyClientsOfDisconnectedUser(e.ClientUser);
                    break;

                case MessageNumber.ConversationRequest:
                    if (CheckConversationIsValid((ConversationRequest) message))
                    {
                        Conversation conversation = CreateConversationEntity((ConversationRequest) message);
                        AddConversationToRepository(conversation);
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
            ClientHandler clientHandler = clientHandlers.FindClientHandlerByUserId(clientUser.UserId);
            clientHandler.SendMessage(userSnapshot);
        }

        private static bool CheckConversationIsValid(ConversationRequest conversationRequest)
        {
            return conversationRequest.Conversation.FirstParticipantUserId != conversationRequest.Conversation.SecondParticipantUserId;
        }

        private Conversation CreateConversationEntity(ConversationRequest conversationRequest)
        {
            return new Conversation(conversationIDGenerator.CreateConversationId(),
                conversationRequest.Conversation.FirstParticipantUserId,
                conversationRequest.Conversation.SecondParticipantUserId);
        }

        private void AddConversationToRepository(Conversation conversation)
        {
            conversationRepository.AddConversation(conversation);
        }

        private void SendConversationNotificationToClients(Conversation conversation)
        {
            var conversationNotification = new ConversationNotification(conversation);
            ClientHandler firstParticipantClientHandler = clientHandlers.FindClientHandlerByUserId(conversation.FirstParticipantUserId);
            firstParticipantClientHandler.SendMessage(conversationNotification);

            ClientHandler secondParticipantClientHandler = clientHandlers.FindClientHandlerByUserId(conversation.SecondParticipantUserId);
            secondParticipantClientHandler.SendMessage(conversationNotification);
        }

        private Contribution CreateContributionEntity(ContributionRequest contributionRequest)
        {
            return new Contribution(
                contributionIDGenerator.CreateConversationId(),
                contributionRequest.SenderID,
                contributionRequest.Message,
                contributionRequest.ConversationID);
        }

        private void AddContributionToRepository(Contribution contribution)
        {
            contributionRepository.AddContribution(contribution);
        }

        private void SendContributionNotificationToParticipants(Contribution contribution)
        {
            var contributionNotification = new ContributionNotification(contribution);
            Conversation contributionConversation = conversationRepository.FindConversationById(contribution.ConversationId);

            ClientHandler firstParticipantClientHandler = clientHandlers.FindClientHandlerByUserId(contributionConversation.FirstParticipantUserId);
            firstParticipantClientHandler.SendMessage(contributionNotification);

            ClientHandler secondParticipantClientHandler = clientHandlers.FindClientHandlerByUserId(contributionConversation.SecondParticipantUserId);
            secondParticipantClientHandler.SendMessage(contributionNotification);
        }

        private void RemoveClientHandler(User clientUser)
        {
            ClientHandler disconnectedUser = null;

            foreach (ClientHandler clientHandler in clientHandlers.Where(clientHandler => clientHandler.ClientUser.UserId == clientUser.UserId))
            {
                disconnectedUser = clientHandler;
            }

            if (disconnectedUser != null)
            {
                clientHandlers.Remove(disconnectedUser);
                Log.Info("User with id " + clientUser.UserId + " logged out. Removing from Server's ClientHandler list");
            }
        }

        private void NotifyClientsOfDisconnectedUser(User disconnectedUser)
        {
            var userNotification = new UserNotification(disconnectedUser, NotificationType.Delete);

            foreach (ClientHandler clientHandler in clientHandlers)
            {
                clientHandler.SendMessage(userNotification);
            }
        }
    }
}