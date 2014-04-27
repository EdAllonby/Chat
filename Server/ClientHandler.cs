using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using log4net;
using SharedClasses;
using SharedClasses.Domain;
using SharedClasses.Protocol;

namespace Server
{
    public sealed class ClientHandler
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (Server));

        private readonly IList<ConnectedClient> connectedClients = new List<ConnectedClient>();
        private readonly ContributionIDGenerator contributionIDGenerator = new ContributionIDGenerator();
        private readonly ContributionRepository contributionRepository = new ContributionRepository();
        private readonly ConversationIDGenerator conversationIDGenerator = new ConversationIDGenerator();
        private readonly ConversationRepository conversationRepository = new ConversationRepository();

        private readonly MessageReceiver messageReceiver = new MessageReceiver();
        private readonly SerialiserFactory serialiserFactory = new SerialiserFactory();
        private readonly UserIDGenerator userIDGenerator = new UserIDGenerator();

        private readonly UserRepository userRepository = new UserRepository();
        private int totalListenerThreads = 1;

        public ClientHandler()
        {
            Log.Info("New client handler created");
            messageReceiver.OnNewMessage += NewMessageReceived;
        }

        public void CreateListenerThreadForClient(TcpClient client)
        {
            LoginRequest clientLogin = GetClientLoginCredentials(client);

            User newConnectedUser = CreateNewUserEntity(clientLogin);

            AddUserToRepository(newConnectedUser);

            NotifyClientsOfNewUser(newConnectedUser);

            var newClient = new ConnectedClient(client, newConnectedUser);

            AddConnectedClient(newClient);

            SendLoginResponseMessage(newClient);

            var messageListenerThread = new Thread(() => messageReceiver.ReceiveMessages(newClient))
            {
                Name = "ReceiveMessageThread" + totalListenerThreads
            };
            messageListenerThread.Start();
            totalListenerThreads++;
        }

        private LoginRequest GetClientLoginCredentials(TcpClient client)
        {
            var messageIdentifierSerialiser = new MessageIdentifierSerialiser();
            int messageIdentifier = messageIdentifierSerialiser.DeserialiseMessageIdentifier(client.GetStream());
            ISerialiser serialiser = serialiserFactory.GetSerialiser(messageIdentifier);
            var loginRequest = (LoginRequest) serialiser.Deserialise(client.GetStream());
            return loginRequest;
        }

        private User CreateNewUserEntity(LoginRequest clientLogin)
        {
            return new User(clientLogin.UserName, userIDGenerator.CreateUserId());
        }

        private void AddUserToRepository(User newConnectedUser)
        {
            userRepository.AddUser(newConnectedUser);
        }

        private void SendLoginResponseMessage(ConnectedClient client)
        {
            var loginResponse = new LoginResponse(client.User);
            SendMessage(loginResponse, client.User.UserId);
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
                    SendUserSnapshot(e.ConnectedClient);
                    break;

                case MessageNumber.ClientDisconnection:
                    RemoveConnectedClient(e.ConnectedClient);
                    NotifyClientsOfDisconnectedUser(e.ConnectedClient.User);
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
            SendMessage(conversationNotification, conversation.FirstParticipantUserId);
            SendMessage(conversationNotification, conversation.SecondParticipantUserId);
        }

        private void SendMessage(IMessage message, int userId)
        {
            ISerialiser messageSerialiser = serialiserFactory.GetSerialiser(message.Identifier);
            TcpClient client = connectedClients.FindConnectedClientByUserId(userId).TcpClient;
            messageSerialiser.Serialise(message, client.GetStream());
            Log.Debug("Sent message with identifier " + message.Identifier + " to user with id " + userId);
        }

        private void SendUserSnapshot(ConnectedClient connectedClient)
        {
            IList<User> currentUsers = connectedClients.Select(client => client.User).ToList();
            var userSnapshot = new UserSnapshot(currentUsers);
            SendMessage(userSnapshot, connectedClient.User.UserId);
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
            SendMessage(contributionNotification, contributionConversation.FirstParticipantUserId);
            SendMessage(contributionNotification, contributionConversation.SecondParticipantUserId);
        }

        private void NotifyClientsOfNewUser(User newUser)
        {
            foreach (ConnectedClient client in connectedClients)
            {
                var userNotification = new UserNotification(newUser, NotificationType.Create);
                SendMessage(userNotification, client.User.UserId);
            }
        }

        private void NotifyClientsOfDisconnectedUser(User disconnectedUser)
        {
            foreach (ConnectedClient client in connectedClients)
            {
                var userNotification = new UserNotification(disconnectedUser, NotificationType.Delete);
                SendMessage(userNotification, client.User.UserId);
            }
        }

        private void AddConnectedClient(ConnectedClient client)
        {
            connectedClients.Add(client);
            Log.Info("Added client to connectedClients list");
        }

        private void RemoveConnectedClient(ConnectedClient client)
        {
            ConnectedClient disconnectedClient = null;

            foreach (
                ConnectedClient connectedClient in
                    connectedClients.Where(connectedClient => connectedClient.User.Username == client.User.Username))
            {
                disconnectedClient = connectedClient;
            }

            if (disconnectedClient != null)
            {
                connectedClients.Remove(disconnectedClient);
                Log.Info("User " + client.User.Username + " logged out. Removing from connectedClients list");
            }
        }
    }
}