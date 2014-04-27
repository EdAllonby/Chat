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

        private static readonly ILog Log = LogManager.GetLogger(typeof (Client));

        private static Client uniqueClientInstance;

        private readonly MessageReceiver messageReceiver = new MessageReceiver();
        private readonly SerialiserFactory serialiserFactory = new SerialiserFactory();

        private readonly IPAddress targetAddress;
        private readonly int targetPort;

        private ConnectedClient client;

        #region Entity Repositories
        
        private readonly ConversationRepository conversationRepository = new ConversationRepository();
        private readonly ContributionRepository contributionRepository = new ContributionRepository();
        private readonly UserRepository userRepository = new UserRepository();
        
        #endregion

        private Client(string userName, IPAddress targetAddress, int targetPort)
        {
            UserName = userName;
            this.targetAddress = targetAddress;
            this.targetPort = targetPort;
            ConnectToServer();
        }

        public ConversationRepository ConversationRepository
        {
            get { return conversationRepository; }
        }

        public ContributionRepository ContributionRepository
        {
            get { return contributionRepository; }
        }

        public UserRepository UserRepository
        {
            get { return userRepository; }
        }

        public string UserName { get; private set; }

        public event UserListHandler OnNewUser = delegate { };
        public event NewConversationHandler OnNewConversationNotification = delegate { };
        public event NewcontributionNotification OnNewContribution = delegate { };

        private void NotifyClientOfUserChange()
        {
            // TODO: Make this work
            OnNewUser(UserRepository.UsersIndexedById.Values.ToList(), EventArgs.Empty);
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
            var clientContribution = new ContributionRequest(conversationID, client.User.UserId, message);
            ISerialiser serialiser = serialiserFactory.GetSerialiser<ContributionRequest>();
            serialiser.Serialise(clientContribution, client.TcpClient.GetStream());
        }

        public void SendConversationRequest(int receiverId)
        {
            var conversation = new Conversation(client.User.UserId, receiverId);
            var conversationRequest = new ConversationRequest(conversation);
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
                case MessageNumber.ContributionNotification:
                    Contribution contribution = CreateContribution((ContributionNotification) message);
                    AddContributionToRepository(contribution);
                    NotifyClientOfNewNotification(contribution);
                    break;

                case MessageNumber.UserNotification:
                    UpdateUserRepository((UserNotification) message);
                    NotifyClientOfUserChange();
                    break;

                case MessageNumber.UserSnapshot:
                    AddUserListToRepository((UserSnapshot) message);
                    break;

                case MessageNumber.ConversationNotification:
                    AddConversationToRepository((ConversationNotification) message);
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

        private void AddConversationToRepository(ConversationNotification conversationNotification)
        {
            conversationRepository.AddConversation(conversationNotification.Conversation);
            OnNewConversationNotification(conversationNotification.Conversation);
        }

        private static Contribution CreateContribution(ContributionNotification contributionNotification)
        {
            return contributionNotification.Contribution;
        }

        private void AddContributionToRepository(Contribution contribution)
        {
            contributionRepository.AddContribution(contribution);
        }

        private void AddUserListToRepository(UserSnapshot userSnapshot)
        {
            foreach (User user in userSnapshot.Users)
            {
                userRepository.AddUser(user);
            }

            OnNewUser(userRepository.UsersIndexedById.Values.ToList(), null);
        }

        private void UpdateUserRepository(UserNotification userNotification)
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
            userRepository.AddUser(userNotification.User);
        }

        private void RemoveUser(UserNotification userNotification)
        {
            userRepository.RemoveUser(userNotification.User.UserId);
        }

        #region Login Procedure Methods

        private void ConnectToServer()
        {
            TcpClient serverConnection = CreateConnection();

            SendLoginRequest(serverConnection);

            LoginResponse loginResponse = GetLoginResponse(serverConnection);

            client = CreateConnectedClient(loginResponse, serverConnection);

            var messageListenerThread = new Thread(ReceiveMessageListener)
            {
                Name = "ReceiveMessageThread"
            };

            messageListenerThread.Start();

            SendUserSnaphotRequest();
        }

        private TcpClient CreateConnection()
        {
            const int timeoutSeconds = 5;

            Log.Info("Client looking for server");

            var serverConnection = new TcpClient();

            serverConnection.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);

            IAsyncResult asyncResult = serverConnection.BeginConnect(targetAddress.ToString(), targetPort, null, null);
            WaitHandle waitHandle = asyncResult.AsyncWaitHandle;
            try
            {
                if (!asyncResult.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(timeoutSeconds), false))
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

            return serverConnection;
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
            return (LoginResponse) serialiser.Deserialise(serverConnection.GetStream());
        }

        private ConnectedClient CreateConnectedClient(LoginResponse loginResponse, TcpClient serverConnection)
        {
            var connectedClient = new ConnectedClient(serverConnection, loginResponse.User);
            return connectedClient;
        }

        #endregion
    }
}