using System;
using System.Collections.Generic;
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
    /// Delegates TCP work off to the <see cref="ConnectionHandler"/>
    /// </summary>
    public sealed class Client
    {
        public delegate void NewContributionNotificationHandler(Conversation contributions);

        public delegate void NewConversationHandler(Conversation conversation);

        public delegate void UserListHandler(IEnumerable<User> users);

        private static readonly ILog Log = LogManager.GetLogger(typeof (Client));

        private readonly RepositoryFactory repositoryFactory = new RepositoryFactory();

        private readonly SerialiserFactory serialiserFactory = new SerialiserFactory();
        private ConnectionHandler connectionHandler;

        public IEntityRepository<User> UserRepository
        {
            get { return repositoryFactory.GetRepository<User>(); }
        }

        public IEntityRepository<Conversation> ConversationRepository
        {
            get { return repositoryFactory.GetRepository<Conversation>(); }
        }

        public int ClientUserId { get; private set; }

        public string Username { get; private set; }

        public event UserListHandler OnNewUser = delegate { };
        public event NewConversationHandler OnNewConversationNotification = delegate { };
        public event NewContributionNotificationHandler OnNewContribution = delegate { };

        private void NotifyClientOfUserChange()
        {
            OnNewUser(repositoryFactory.GetRepository<User>().GetAllEntities());
            Log.Info("User changed event fired");
        }

        public void ConnectToServer(string username, IPAddress targetAddress, int targetPort)
        {
            Username = username;

            TcpClient tcpClient = CreateConnection(targetAddress, targetPort);

            IMessage userRequest = new LoginRequest(Username);

            SendLoginRequestMessage(userRequest, tcpClient);

            LoginResponse loginResponse = GetLoginResponse(tcpClient);

            ClientUserId = loginResponse.User.UserId;

            connectionHandler = new ConnectionHandler(ClientUserId, tcpClient);

            connectionHandler.SendMessage(new UserSnapshotRequest());

            connectionHandler.OnNewMessage += NewMessageReceived;
        }

        private LoginResponse GetLoginResponse(TcpClient tcpClient)
        {
            var messageIdentifierSerialiser = new MessageIdentifierSerialiser();
            int messageIdentifier = messageIdentifierSerialiser.DeserialiseMessageIdentifier(tcpClient.GetStream());
            ISerialiser serialiser = serialiserFactory.GetSerialiser(messageIdentifier);
            return (LoginResponse) serialiser.Deserialise(tcpClient.GetStream());
        }

        private void SendLoginRequestMessage(IMessage message, TcpClient tcpClient)
        {
            ISerialiser messageSerialiser = serialiserFactory.GetSerialiser(message.Identifier);
            messageSerialiser.Serialise(message, tcpClient.GetStream());
        }

        private static TcpClient CreateConnection(IPAddress targetAddress, int targetPort)
        {
            const int TimeoutSeconds = 5;

            Log.Info("Client looking for server with address: " + targetAddress + " and port: " + targetPort);

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
                    AddContributionToConversation(contributionNotification);
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
                    Log.Warn("Client is not supposed to handle message with identifier: " + message.Identifier);
                    break;
            }
        }

        private void AddContributionToConversation(ContributionNotification contributionNotification)
        {
            Conversation conversation = repositoryFactory.GetRepository<Conversation>()
                .FindEntityByID(contributionNotification.Contribution.ConversationId);

            conversation.AddContribution(contributionNotification);
            OnNewContribution(conversation);
        }

        public void SendContributionRequest(int conversationID, string message)
        {
            var clientContribution = new ContributionRequest(conversationID, ClientUserId, message);
            connectionHandler.SendMessage(clientContribution);
        }

        public void SendConversationRequest(int receiverId)
        {
            var conversationRequest = new ConversationRequest(ClientUserId, receiverId);
            connectionHandler.SendMessage(conversationRequest);
        }

        private void AddConversationToRepository(ConversationNotification conversationNotification)
        {
            repositoryFactory.GetRepository<Conversation>().AddEntity(conversationNotification.Conversation);
            OnNewConversationNotification(conversationNotification.Conversation);
        }

        private void AddUserListToRepository(UserSnapshot userSnapshot)
        {
            repositoryFactory.GetRepository<User>().AddEntities(userSnapshot.Users);
            OnNewUser(repositoryFactory.GetRepository<User>().GetAllEntities());
        }

        private void UpdateUserRepository(UserNotification userNotification)
        {
            switch (userNotification.Notification)
            {
                case NotificationType.Create:
                    repositoryFactory.GetRepository<User>().AddEntity(userNotification.User);
                    break;
                case NotificationType.Delete:
                    repositoryFactory.GetRepository<User>().RemoveEntity(userNotification.User.UserId);
                    break;
            }
        }
    }
}