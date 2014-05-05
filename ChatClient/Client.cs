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

        private readonly ConversationRepository conversationRepository = new ConversationRepository();

        private readonly MessageReceiver messageReceiver = new MessageReceiver();
        private readonly SerialiserFactory serialiserFactory = new SerialiserFactory();
        private readonly UserRepository userRepository = new UserRepository();

        private TcpClient tcpClient;

        public UserRepository UserRepository
        {
            get { return userRepository; }
        }

        public int ClientUserId { get; private set; }

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

            IMessage userRequest = new LoginRequest(new User(UserName));
            SendMessage(userRequest);

            LoginResponse loginResponse = GetLoginResponse();

            GetClientUserId(loginResponse);

            SendMessage(new UserSnapshotRequest());

            messageReceiver.OnNewMessage += NewMessageReceived;

            CreateListenerThreadForClient();
        }

        private LoginResponse GetLoginResponse()
        {
            var messageIdentifierSerialiser = new MessageIdentifierSerialiser();
            int messageIdentifier = messageIdentifierSerialiser.DeserialiseMessageIdentifier(tcpClient.GetStream());
            ISerialiser serialiser = serialiserFactory.GetSerialiser(messageIdentifier);
            return (LoginResponse) serialiser.Deserialise(tcpClient.GetStream());
        }

        private void CreateListenerThreadForClient()
        {
            var messageListenerThread = new Thread(() => messageReceiver.ReceiveMessages(ClientUserId, tcpClient))
            {
                Name = "ReceiveMessageThread" + (totalListenerThreads++)
            };
            messageListenerThread.Start();
        }

        private void SendMessage(IMessage message)
        {
            ISerialiser messageSerialiser = serialiserFactory.GetSerialiser(message.Identifier);
            messageSerialiser.Serialise(message, tcpClient.GetStream());
        }

        private static TcpClient CreateConnection(IPAddress targetAddress, int targetPort)
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

        private void NewMessageReceived(object sender, MessageEventArgs e)
        {
            IMessage message = e.Message;

            switch (message.Identifier)
            {
                case MessageNumber.ContributionNotification:
                    var contributionNotification = (ContributionNotification) message;
                    Contribution contribution = contributionNotification.Contribution;
                    AddContributionToConversation(contribution);
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

                default:
                    Log.Warn("Shared classes assembly does not have a definition for message identifier: " + message.Identifier);
                    break;
            }
        }

        private void AddContributionToConversation(Contribution contribution)
        {
            Conversation conversation = conversationRepository.FindConversationById(contribution.ConversationId);
            conversation.AddContribution(contribution);
        }

        private void GetClientUserId(LoginResponse message)
        {
            ClientUserId = message.User.UserId;
        }

        public void SendConversationContributionRequest(int conversationID, string message)
        {
            var clientContribution = new ContributionRequest(conversationID, ClientUserId, message);
            SendMessage(clientContribution);
        }

        public void SendConversationRequest(int receiverId)
        {
            var conversation = new Conversation(ClientUserId, receiverId);
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