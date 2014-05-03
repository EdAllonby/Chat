using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using log4net;
using SharedClasses;
using SharedClasses.Domain;
using SharedClasses.Message;
using SharedClasses.Serialiser;

namespace ChatClient
{
    /// <summary>
    /// Handles the logic for <see cref="IMessage"/>
    /// </summary>
    public sealed class Client
    {
        public delegate void NewConversationHandler(Conversation conversation);

        public delegate void NewcontributionNotification(Contribution contribution);

        public delegate void UserListHandler(IList<User> users, EventArgs e);

        private static readonly ILog Log = LogManager.GetLogger(typeof (Client));
        private static int totalListenerThreads;

        private readonly ContributionRepository contributionRepository = new ContributionRepository();
        private readonly ConversationRepository conversationRepository = new ConversationRepository();

        private readonly MessageReceiver messageReceiver = new MessageReceiver();
        private readonly SerialiserFactory serialiserFactory = new SerialiserFactory();
        private readonly UserRepository userRepository = new UserRepository();

        private int clientUserId;
        private TcpClient tcpClient;

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
            OnNewUser(UserRepository.UsersIndexedById.Values.ToList(), EventArgs.Empty);
            Log.Info("User changed event fired");
        }

        public void ConnectToServer(string userName, IPAddress targetAddress, int targetPort)
        {
            UserName = userName;

            tcpClient = CreateConnection(targetAddress, targetPort);

            messageReceiver.OnNewMessage += NewMessageReceived;

            CreateListenerThreadForClient();

            SendLoginRequest(tcpClient, UserName);
        }

        private void CreateListenerThreadForClient()
        {
            var messageListenerThread = new Thread(() => messageReceiver.ReceiveMessages(clientUserId, tcpClient))
            {
                Name = "ReceiveMessageThread" + (totalListenerThreads++)
            };
            messageListenerThread.Start();
        }

        public void SendMessage(IMessage message)
        {
            ISerialiser messageSerialiser = serialiserFactory.GetSerialiser(message.Identifier);
            messageSerialiser.Serialise(message, tcpClient.GetStream());
            Log.Debug("Sent message with identifier " + message.Identifier + " to user with id " + clientUserId);
        }

        private TcpClient CreateConnection(IPAddress targetAddress, int targetPort)
        {
            const int TimeoutSeconds = 5;

            Log.Info("Client looking for server");

            var serverConnection = new TcpClient();

            serverConnection.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);

            IAsyncResult asyncResult = serverConnection.BeginConnect(targetAddress.ToString(), targetPort, null, null);
            WaitHandle waitHandle = asyncResult.AsyncWaitHandle;
            try
            {
                if (!asyncResult.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(TimeoutSeconds), false))
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

        private static void SendLoginRequest(TcpClient tcpClient, string username)
        {
            IMessage userRequest = new LoginRequest(new User(username));
            var serialiserFactory = new SerialiserFactory();
            ISerialiser messageSerialiser = serialiserFactory.GetSerialiser(userRequest.Identifier);
            messageSerialiser.Serialise(userRequest, tcpClient.GetStream());
            Log.Debug("Sent message with identifier " + userRequest.Identifier + " to server");
        }

        private static UserNotification GetUserNotification(TcpClient tcpClient)
        {
            var serialiserFactory = new SerialiserFactory();
            var messageIdentifierSerialiser = new MessageIdentifierSerialiser();
            int messageIdentifier = messageIdentifierSerialiser.DeserialiseMessageIdentifier(tcpClient.GetStream());
            ISerialiser serialiser = serialiserFactory.GetSerialiser(messageIdentifier);
            return (UserNotification) serialiser.Deserialise(tcpClient.GetStream());
        }

        private void NewMessageReceived(object sender, MessageEventArgs e)
        {
            IMessage message = e.Message;

            switch (message.Identifier)
            {
                case MessageNumber.ContributionNotification:
                    var contributionNotification = (ContributionNotification) message;
                    Contribution contribution = contributionNotification.Contribution;
                    contributionRepository.AddContribution(contribution);
                    OnNewContribution(contribution);
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

                case MessageNumber.LoginResponse:
                    GetClientUserId((LoginResponse) message);
                    SendMessage(new UserSnapshotRequest());
                    break;

                default:
                    Log.Warn("Shared classes assembly does not have a definition for message identifier: " + message.Identifier);
                    break;
            }
        }

        private void GetClientUserId(LoginResponse message)
        {
            clientUserId = message.User.UserId;
        }

        public void SendConversationContributionRequest(int conversationID, string message)
        {
            var clientContribution = new ContributionRequest(conversationID, clientUserId, message);
            SendMessage(clientContribution);
        }

        public void SendConversationRequest(int receiverId)
        {
            var conversation = new Conversation(clientUserId, receiverId);
            var conversationRequest = new ConversationRequest(conversation);
            SendMessage(conversationRequest);
        }

        private void AddConversationToRepository(ConversationNotification conversationNotification)
        {
            conversationRepository.AddConversation(conversationNotification.Conversation);
            OnNewConversationNotification(conversationNotification.Conversation);
        }

        private void AddUserListToRepository(UserSnapshot userSnapshot)
        {
            userRepository.AddUsers(userSnapshot.Users);

            OnNewUser(userRepository.UsersIndexedById.Values.ToList(), null);
        }

        private void UpdateUserRepository(UserNotification userNotification)
        {
            switch (userNotification.Notification)
            {
                case NotificationType.Create:
                    userRepository.AddUser(userNotification.User);
                    break;
                case NotificationType.Delete:
                    userRepository.RemoveUser(userNotification.User.UserId);
                    break;
            }
        }
    }
}