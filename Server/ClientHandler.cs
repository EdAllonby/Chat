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
        private readonly ConversationFactory conversationFactory = new ConversationFactory();
        private readonly IList<Conversation> conversations = new List<Conversation>();

        private readonly MessageReceiver messageReceiver = new MessageReceiver();
        private readonly SerialiserFactory serialiserFactory = new SerialiserFactory();
        private readonly UserFactory userFactory = new UserFactory();

        private int totalListenerThreads = 1;

        public ClientHandler()
        {
            Log.Info("New client handler created");
            messageReceiver.OnNewMessage += NewMessageReceived;
        }

        public void CreateListenerThreadForClient(TcpClient client)
        {
            LoginRequest clientLogin = ClientLoginCredentials(client);

            if (clientLogin != null)
            {
                User newConnectedUser = userFactory.CreateUser(clientLogin.UserName);

                var newClient = new ConnectedClient(client, newConnectedUser);

                NotifyClientsOfNewUser(newConnectedUser);

                AddConnectedClient(newClient);

                SendLoginResponseMessage(newClient);

                var messageListenerThread = new Thread(() => messageReceiver.ReceiveMessages(newClient))
                {
                    Name = "ReceiveMessageThread" + totalListenerThreads
                };
                messageListenerThread.Start();
                totalListenerThreads++;
            }
            else
            {
                Log.Error("User didn't send Login Request message as first message.");
            }
        }

        private void SendLoginResponseMessage(ConnectedClient client)
        {
            ISerialiser loginResponseSerialiser = serialiserFactory.GetSerialiser<LoginResponse>();
            var loginResponse = new LoginResponse(client.User.ID);
            loginResponseSerialiser.Serialise(loginResponse, client.TcpClient.GetStream());
        }

        private LoginRequest ClientLoginCredentials(TcpClient client)
        {
            var messageIdentifierSerialiser = new MessageIdentifierSerialiser();
            int messageIdentifier = messageIdentifierSerialiser.DeserialiseMessageIdentifier(client.GetStream());
            ISerialiser serialiser = serialiserFactory.GetSerialiser(messageIdentifier);
            IMessage message = serialiser.Deserialise(client.GetStream());
            return message as LoginRequest;
        }

        private void NewMessageReceived(object sender, MessageEventArgs e)
        {
            IMessage message = e.Message;

            switch (message.Identifier)
            {
                case MessageNumber.ContributionRequest:
                    Contribution contribution = CreateContribution((ContributionRequest) message);
                    AddContributionToConversation(contribution);
                    SendContributionNotificationToParticipants(contribution);
                    break;
                case MessageNumber.ContributionNotification:
                    Log.Warn("Client should not be sending a User Notification Message if following protocol");
                    break;
                case MessageNumber.LoginRequest:
                    Log.Warn("Client should not be sending a Login Request Message if following protocol");
                    break;
                case MessageNumber.UserNotification:
                    Log.Warn("Client should not be sending a User Notification Message if following protocol");
                    break;
                case MessageNumber.UserSnapshotRequest:
                    SendUserSnapshot(e.ConnectedClient);
                    break;
                case MessageNumber.UserSnapshot:
                    Log.Warn("Client should not be sending a User Snapshot Message if following protocol");
                    break;
                case MessageNumber.ClientDisconnection:
                    RemoveConnectedClient(e.ConnectedClient);
                    NotifyClientsOfDisconnectedUser(e.ConnectedClient.User);
                    break;
                case MessageNumber.ConversationRequest:
                    var conversationRequest = (ConversationRequest) message;
                    bool isExistingConversation = CheckConversationIsUnique(conversationRequest);
                    if (!isExistingConversation)
                    {
                        Conversation conversation = AddConversation(conversationRequest);
                        SendConversationNotification(conversation);
                    }
                    break;
                case MessageNumber.ConversationNotification:
                    Log.Warn("Client should not be sending a Conversation Notification Message if following protocol");
                    break;
                default:
                    Log.Warn("Shared classes assembly does not have a definition for message identifier: " + message.Identifier);
                    break;
            }
        }

        private bool CheckConversationIsUnique(ConversationRequest message)
        {
            User firstParticipant = ConnectedClient.FindByUserID(connectedClients, message.SenderID).User;
            User secondParticipant = ConnectedClient.FindByUserID(connectedClients, message.ReceiverID).User;

            bool isExistingConversation = conversations.Any(conversation => (conversation.FirstParticipant.Equals(firstParticipant) && conversation.SecondParticipant.Equals(secondParticipant) ||
                                                                             conversation.FirstParticipant.Equals(secondParticipant) && conversation.SecondParticipant.Equals(firstParticipant)));

            if (firstParticipant.Equals(secondParticipant))
            {
                Log.Warn("Can't make conversation with yourself");
                return true;
            }

            if (isExistingConversation)
            {
                Log.Warn("Conversation already created");
            }

            return isExistingConversation;
        }

        private Conversation AddConversation(ConversationRequest conversationRequest)
        {
            User firstParticipant = ConnectedClient.FindByUserID(connectedClients, conversationRequest.SenderID).User;
            User secondParticipant = ConnectedClient.FindByUserID(connectedClients, conversationRequest.ReceiverID).User;

            Conversation newConversation = conversationFactory.CreateConversation(firstParticipant, secondParticipant);
            conversations.Add(newConversation);
            return newConversation;
        }

        private void SendConversationNotification(Conversation conversation)
        {
            var conversationNotification = new ConversationNotification(conversation);

            ISerialiser conversationNotificationSerialiser = serialiserFactory.GetSerialiser<ConversationNotification>();

            ConnectedClient senderClient = ConnectedClient.FindByUserID(connectedClients, conversation.FirstParticipant.ID);
            conversationNotificationSerialiser.Serialise(conversationNotification, senderClient.TcpClient.GetStream());

            ConnectedClient receiverClient = ConnectedClient.FindByUserID(connectedClients, conversation.SecondParticipant.ID);
            conversationNotificationSerialiser.Serialise(conversationNotification, receiverClient.TcpClient.GetStream());
        }

        private Contribution CreateContribution(ContributionRequest contributionRequest)
        {
            var contribution = new Contribution(ConnectedClient.FindByUserID(connectedClients, contributionRequest.SenderID).User, contributionRequest.Message);

            Conversation targetedConversation = conversations.FirstOrDefault(x => x.ID == contributionRequest.ConversationID);

            contribution.Conversation = targetedConversation;
            return contribution;
        }

        private static void AddContributionToConversation(Contribution contribution)
        {
            contribution.Conversation.Contributions.Add(contribution);
        }

        private void SendContributionNotificationToParticipants(Contribution message)
        {
            ISerialiser contributionNotificationSerialiser = serialiserFactory.GetSerialiser<ContributionNotification>();

            var contributionNotification = new ContributionNotification(message.Conversation.ID, message.Contributor.ID, message.Text);

            ConnectedClient senderClient = ConnectedClient.FindByUserID(connectedClients, message.Conversation.FirstParticipant.ID);
            contributionNotificationSerialiser.Serialise(contributionNotification, senderClient.TcpClient.GetStream());

            ConnectedClient receiverClient = ConnectedClient.FindByUserID(connectedClients, message.Conversation.SecondParticipant.ID);
            contributionNotificationSerialiser.Serialise(contributionNotification, receiverClient.TcpClient.GetStream());
        }

        private void SendUserSnapshot(ConnectedClient connectedClient)
        {
            ISerialiser userSnapshotSerialiser = serialiserFactory.GetSerialiser<UserSnapshot>();

            IList<User> currentUsers = connectedClients.Select(client => client.User).ToList();
            var userSnapshot = new UserSnapshot(currentUsers);

            userSnapshotSerialiser.Serialise(userSnapshot, connectedClient.TcpClient.GetStream());
        }

        private void NotifyClientsOfNewUser(User newUser)
        {
            ISerialiser messageSerialiser = serialiserFactory.GetSerialiser<UserNotification>();

            foreach (ConnectedClient client in connectedClients)
            {
                NetworkStream clientStream = client.TcpClient.GetStream();
                var userNotification = new UserNotification(newUser, NotificationType.Create);
                messageSerialiser.Serialise(userNotification, clientStream);
            }
        }

        private void NotifyClientsOfDisconnectedUser(User disconnectedUser)
        {
            ISerialiser messageSerialiser = serialiserFactory.GetSerialiser<UserNotification>();

            foreach (ConnectedClient client in connectedClients)
            {
                NetworkStream clientStream = client.TcpClient.GetStream();
                var userNotification = new UserNotification(disconnectedUser, NotificationType.Delete);
                messageSerialiser.Serialise(userNotification, clientStream);
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
                    connectedClients.Where(connectedClient => connectedClient.User.UserName == client.User.UserName))
            {
                disconnectedClient = connectedClient;
            }

            if (disconnectedClient != null)
            {
                connectedClients.Remove(disconnectedClient);
                Log.Info("User " + client.User.UserName + " logged out. Removing from connectedClients list");
            }
        }
    }
}