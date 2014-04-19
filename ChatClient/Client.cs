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
    internal sealed class Client
    {
        public delegate void NewContributionHandler(string contribution, EventArgs e);

        public delegate void UserListHandler(IList<User> users, EventArgs e);

        private const int LogonTimeout = 5;

        private static readonly ILog Log = LogManager.GetLogger(typeof (Client));

        private static Client uniqueClientInstance;

        private readonly MessageReceiver messageReceiver = new MessageReceiver();
        private readonly SerialiserFactory serialiserFactory = new SerialiserFactory();

        private readonly IPAddress targetAddress;
        private readonly int targetPort;

        private ConnectedClient client;
        private IList<User> connectedUsers = new List<User>();

        private Client(string userName, IPAddress targetAddress, int targetPort)
        {
            UserName = userName;
            this.targetAddress = targetAddress;
            this.targetPort = targetPort;
            ConnectToServer();
        }

        public string UserName { get; private set; }

        public event NewContributionHandler OnNewContributionNotification = delegate { };
        public event UserListHandler OnNewUser = delegate { };

        private void NotifyClientOfUserChange()
        {
            OnNewUser(connectedUsers, EventArgs.Empty);
            Log.Info("User changed event fired");
        }

        private void NotifyClientOfContributionNotification(ContributionNotification contributionNotification)
        {
            Log.Info("Server sent: " + contributionNotification.Contribution.GetMessage());
            OnNewContributionNotification(contributionNotification.Contribution.GetMessage(), EventArgs.Empty);
            Log.Info("New contribution notification event fired");
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

        public void SendContributionRequestMessage(string message)
        {
            var clientContribution = new ContributionRequest(new Contribution(message));
            ISerialiser serialiser = serialiserFactory.GetSerialiser<ContributionRequest>();
            serialiser.Serialise(clientContribution, client.TcpClient.GetStream());
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
                    NotifyClientOfContributionNotification((ContributionNotification) message);
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
                default:
                    Log.Warn("Shared classes assembly does not have a definition for message identifier: " + message.Identifier);
                    break;
            }
        }

        private void ListCurrentUsers(UserSnapshot userSnapshot)
        {
            connectedUsers = userSnapshot.Users;

            Log.Info("Currently connected users: ");
            foreach (User user in userSnapshot.Users)
            {
                Log.Info(user.UserName);
            }

            OnNewUser(connectedUsers, null);
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
            connectedUsers.Add(userNotification.User);
            Log.Info("New user logged in successfully, currently connected users: ");
            foreach (User user in connectedUsers)
            {
                Log.Info("User: " + user.UserName);
            }
        }

        private void RemoveUser(UserNotification userNotification)
        {
            User disconnectedUser = null;

            foreach (User user in connectedUsers.Where(user => user.UserName == userNotification.User.UserName))
            {
                disconnectedUser = user;
            }

            if (disconnectedUser != null)
            {
                connectedUsers.Remove(disconnectedUser);
                Log.Info("User " + userNotification.User.UserName + " logged out. Removing from connectedClients list");
            }
        }

        #region Login Procedure Methods

        private void ConnectToServer()
        {
            Log.Info("Client looking for server");

            var serverConnection = new TcpClient();

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