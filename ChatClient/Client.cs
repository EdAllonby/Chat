using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using log4net;
using SharedClasses;
using SharedClasses.Domain;
using SharedClasses.Protocol;

namespace ChatClient
{
    public sealed class Client
    {
        public delegate void NewConversationHandler(Conversation conversation);

        public delegate void NewcontributionNotification(Contribution contribution);

        public delegate void UserListHandler(IList<User> users, EventArgs e);

        private const int LogonTimeout = 5;

        private static readonly ILog Log = LogManager.GetLogger(typeof (Client));

        private static Client uniqueClientInstance;

        private readonly MessageReceiver messageReceiver = new MessageReceiver();
        private readonly SerialiserFactory serialiserFactory = new SerialiserFactory();

        private readonly IPAddress targetAddress;
        private readonly int targetPort;

        private ConnectedClient client;

        private Client(string userName, IPAddress targetAddress, int targetPort)
        {
            UserName = userName;
            this.targetAddress = targetAddress;
            this.targetPort = targetPort;
            Conversations = new List<Conversation>();
            ConnectToServer();
        }

        public IList<User> ConnectedUsers { get; private set; }

        public IList<Conversation> Conversations { get; private set; }

        public string UserName { get; private set; }

        public event UserListHandler OnNewUser = delegate { };
        public event NewConversationHandler OnNewConversationNotification = delegate { };
        public event NewcontributionNotification OnNewContribution = delegate { };

        private void NotifyClientOfUserChange()
        {
            OnNewUser(ConnectedUsers, EventArgs.Empty);
            Log.Info("User changed event fired");
        }

        public static Client GetInstance(string username, IPAddress targetAddress, int targetPort)
        {
            return uniqueClientInstance ?? (uniqueClientInstance = new Client(username, targetAddress, targetPort));
        }

        public static Client GetInstance()
        {
            if (uniqueClientInstance == null)
            {
                throw new NullReferenceException("Can't instantiate Client Class with this method. Use overloaded GetInstance() method");
            }

            return uniqueClientInstance;
        }

        private void SendUserSnaphotRequest()
        {
            ISerialiser userSnapshotRequestSerialiser = serialiserFactory.GetSerialiser<UserSnapshotRequest>();
            var userSnapshotRequest = new UserSnapshotRequest();
            userSnapshotRequestSerialiser.Serialise(userSnapshotRequest, client.TcpClient.GetStream());
        }

        public void SendConversationContributionRequest(int conversationID, string message)
        {
            var clientContribution = new ContributionRequest(conversationID, client.User.ID, message);
            ISerialiser serialiser = serialiserFactory.GetSerialiser<ContributionRequest>();
            serialiser.Serialise(clientContribution, client.TcpClient.GetStream());
        }

        public void SendConversationRequest(int receiverID)
        {
            var conversationRequest = new ConversationRequest(client.User.ID, receiverID);
            ISerialiser serialiser = serialiserFactory.GetSerialiser<ConversationRequest>();
            serialiser.Serialise(conversationRequest, client.TcpClient.GetStream());
        }

        private void ReceiveMessageListener()
        {
            Log.Info("Message listener thread started");
            messageReceiver.OnNewMessage += NewMessageReceived;
            messageReceiver.ReceiveMessages(client);
        }

        private void NewMessageReceived(object sender, MessageEventArgs e)
        {
            IMessage message = e.Message;

            switch (message.Identifier)
            {
                case MessageNumber.ContributionRequest: //Contribution Request
                    Log.Warn("Server shouldn't be sending a ContributionRequest message to a client if following protocol");
                    break;
                case MessageNumber.ContributionNotification: //Contribution Notification
                    Contribution contribution = CreateContribution((ContributionNotification) message);
                    AddContributionToConversation(contribution);
                    NotifyClientOfNewNotification(contribution);
                    break;
                case MessageNumber.LoginRequest: //Login Request
                    Log.Warn("Server shouldn't be sending a LoginRequest message to a client if following protocol");
                    break;
                case MessageNumber.UserNotification: //User Notification
                    UpdateUserCollections((UserNotification) message);
                    NotifyClientOfUserChange();
                    break;
                case MessageNumber.UserSnapshotRequest: //User Snapshot Request
                    Log.Warn("Server shouldn't be sending a User Snapshot Request message to a client if following protocol");
                    break;
                case MessageNumber.UserSnapshot: //User Snapshot
                    ListCurrentUsers((UserSnapshot) message);
                    break;
                case MessageNumber.ConversationRequest:
                    Log.Warn("Server shouldn't be sending a Conversation Request message to a client if following protocol");
                    break;
                case MessageNumber.ConversationNotification:
                    AddConversation((ConversationNotification) message);
                    break;
                default:
                    Log.Warn("Shared classes assembly does not have a definition for message identifier: " + message.Identifier);
                    break;
            }
        }

        private void NotifyClientOfNewNotification(Contribution contribution)
        {
            OnNewContribution(contribution);
        }

        private void AddConversation(ConversationNotification message)
        {
            User firstParticipant = ConnectedUsers.FindByUserID(message.SenderID);
            User secondParticipant = ConnectedUsers.FindByUserID(message.ReceiverID);

            var conversation = new Conversation(message.ConversationID, firstParticipant, secondParticipant);

            Conversations.Add(conversation);
            OnNewConversationNotification(conversation);
        }

        private Contribution CreateContribution(ContributionNotification contributionNotification)
        {
            Conversation targetedConversation = Conversations.FirstOrDefault(x => x.ID == contributionNotification.ConversationID);
            var contribution = new Contribution(contributionNotification.ContributionID, ConnectedUsers.FindByUserID(contributionNotification.SenderID), contributionNotification.Message, targetedConversation);
            return contribution;
        }

        private static void AddContributionToConversation(Contribution contribution)
        {
            contribution.Conversation.Contributions.Add(contribution);
        }

        private void ListCurrentUsers(UserSnapshot userSnapshot)
        {
            ConnectedUsers = userSnapshot.Users;

            Log.Info("Currently connected users: ");
            foreach (User user in userSnapshot.Users)
            {
                Log.Info(user.UserName);
            }

            OnNewUser(ConnectedUsers, null);
        }

        private void UpdateUserCollections(UserNotification userNotification)
        {
            switch (userNotification.Notification)
            {
                case NotificationType.Create:
                    AddUser(userNotification);
                    break;
                case NotificationType.Delete:
                    RemoveUser(userNotification);
                    break;
            }
        }

        private void AddUser(UserNotification userNotification)
        {
            ConnectedUsers.Add(userNotification.User);
            Log.Info("New user logged in successfully, currently connected users: ");
            foreach (User user in ConnectedUsers)
            {
                Log.Info("User: " + user.UserName);
            }
        }

        private void RemoveUser(UserNotification userNotification)
        {
            User disconnectedUser = null;

            foreach (User user in ConnectedUsers.Where(user => user.UserName == userNotification.User.UserName))
            {
                disconnectedUser = user;
            }

            if (disconnectedUser != null)
            {
                ConnectedUsers.Remove(disconnectedUser);
                Log.Info("User " + userNotification.User.UserName + " logged out. Removing from connectedClients list");
            }
        }

        #region Login Procedure Methods

        private void ConnectToServer()
        {
            Log.Info("Client looking for server");

            var serverConnection = new TcpClient();

            serverConnection.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);

            IAsyncResult asyncResult = serverConnection.BeginConnect(targetAddress.ToString(), targetPort, null, null);
            WaitHandle waitHandle = asyncResult.AsyncWaitHandle;
            try
            {
                if (!asyncResult.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(LogonTimeout), false))
                {
                    serverConnection.Close();
                    throw new TimeoutException();
                }

                serverConnection.EndConnect(asyncResult);
            }
            finally
            {
                waitHandle.Close();
            }

            Log.Info("Client found server, connection created");

            SendLoginRequest(serverConnection);

            LoginResponse loginResponse = GetLoginResponse(serverConnection);

            if (loginResponse != null)
            {
                client = CreateConnectedClient(loginResponse, serverConnection);

                var messageListenerThread = new Thread(ReceiveMessageListener)
                {
                    Name = "ReceiveMessageThread"
                };
                messageListenerThread.Start();

                SendUserSnaphotRequest();
            }
            else
            {
                throw new Exception("Client did not receive expected LoginResponse message");
            }
        }

        private void SendLoginRequest(TcpClient serverConnection)
        {
            ISerialiser loginRequestSerialiser = serialiserFactory.GetSerialiser<LoginRequest>();
            var loginRequest = new LoginRequest(UserName);
            loginRequestSerialiser.Serialise(loginRequest, serverConnection.GetStream());
        }

        private LoginResponse GetLoginResponse(TcpClient serverConnection)
        {
            var messageIdentifierSerialiser = new MessageIdentifierSerialiser();
            int messageIdentifier = messageIdentifierSerialiser.DeserialiseMessageIdentifier(serverConnection.GetStream());
            ISerialiser serialiser = serialiserFactory.GetSerialiser(messageIdentifier);
            IMessage message = serialiser.Deserialise(serverConnection.GetStream());
            var loginResponse = message as LoginResponse;
            return loginResponse;
        }

        private ConnectedClient CreateConnectedClient(LoginResponse loginResponse, TcpClient serverConnection)
        {
            var user = new User(UserName, loginResponse.UserID);
            var connectedClient = new ConnectedClient(serverConnection, user);
            return connectedClient;
        }

        #endregion
    }
}