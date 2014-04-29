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
    /// <summary>
    /// Handles the logic for incoming <see cref="IMessage" /> passed from ClientHandler
    /// </summary>
    public sealed class Client
    {
        public delegate void NewConversationHandler(Conversation conversation);

        public delegate void NewcontributionNotification(Contribution contribution);

        public delegate void UserListHandler(IList<User> users, EventArgs e);

        private static readonly ILog Log = LogManager.GetLogger(typeof (Client));
        private static Client uniqueClientInstance;

        private readonly ContributionRepository contributionRepository = new ContributionRepository();
        private readonly ConversationRepository conversationRepository = new ConversationRepository();
        private readonly IPAddress targetAddress;
        private readonly int targetPort;
        private readonly UserRepository userRepository = new UserRepository();
        private ClientHandler clientHandler;

        private Client(string userName, IPAddress targetAddress, int targetPort)
        {
            UserName = userName;
            this.targetAddress = targetAddress;
            this.targetPort = targetPort;
            ConnectToServer();
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
                throw new NullReferenceException(
                    "Can't instantiate Client Class with this method. Use overloaded GetInstance() method");
            }

            return uniqueClientInstance;
        }

        private void ConnectToServer()
        {
            TcpClient tcpClient = CreateConnection();

            SendUserRequest(tcpClient, UserName);

            UserNotification userNotification = GetUserNotification(tcpClient);

            User clientUser = userNotification.User;

            clientHandler = new ClientHandler(clientUser, tcpClient);

            clientHandler.SendMessage(new UserSnapshotRequest());

            clientHandler.OnNewMessage += NewMessageReceived;

            clientHandler.CreateListenerThreadForClient();
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

        private static void SendUserRequest(TcpClient tcpClient, string username)
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

        public void SendConversationContributionRequest(int conversationID, string message)
        {
            var clientContribution = new ContributionRequest(conversationID, clientHandler.ClientUser.UserId, message);
            clientHandler.SendMessage(clientContribution);
        }

        public void SendConversationRequest(int receiverId)
        {
            var conversation = new Conversation(clientHandler.ClientUser.UserId, receiverId);
            var conversationRequest = new ConversationRequest(conversation);
            clientHandler.SendMessage(conversationRequest);
        }

        private void NotifyClientOfNewNotification(Contribution contribution)
        {
            OnNewContribution(contribution);
        }

        private void AddUserToRepository(UserNotification userNotification)
        {
            userRepository.AddUser(userNotification.User);
        }

        private void RemoveUserFromRepository(UserNotification userNotification)
        {
            userRepository.RemoveUser(userNotification.User.UserId);
        }

        private void AddConversationToRepository(ConversationNotification conversationNotification)
        {
            conversationRepository.AddConversation(conversationNotification.Conversation);
            OnNewConversationNotification(conversationNotification.Conversation);
        }

        private void AddContributionToRepository(Contribution contribution)
        {
            contributionRepository.AddContribution(contribution);
        }

        private static Contribution CreateContribution(ContributionNotification contributionNotification)
        {
            return contributionNotification.Contribution;
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
                    AddUserToRepository(userNotification);
                    break;
                case NotificationType.Delete:
                    RemoveUserFromRepository(userNotification);
                    break;
            }
        }
    }
}