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
    public class ClientHandler
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (Server));

        private readonly IList<TcpClient> connectedClients = new List<TcpClient>();

        private readonly IList<User> connectedUsers = new List<User>();

        private readonly MessageReceiver messageReceiver = new MessageReceiver();
        private readonly SerialiserFactory serialiserFactory = new SerialiserFactory();

        private int totalListenerThreads = 1;

        public ClientHandler()
        {
            Log.Info("New client handler created");
            messageReceiver.OnNewMessage += NewMessageReceived;
        }

        public void CreateListenerThreadForClient(TcpClient client)
        {
            AddConnectedClient(client);
            NetworkStream stream = client.GetStream();
            Log.Info("Stream with client established");


            var messageListenerThread = new Thread(() => messageReceiver.ReceiveMessages(stream))
            {
                Name = "MessageListenerThread" + totalListenerThreads
            };

            totalListenerThreads++;

            messageListenerThread.Start();
        }

        private void NewMessageReceived(object sender, MessageEventArgs e)
        {
            IMessage message = e.Message;

            switch (message.Identifier)
            {
                case MessageNumber.ContributionRequest:
                    var contributionRequest = (ContributionRequest) message;
                    SendContributionNotificationToClients(contributionRequest);
                    break;
                case MessageNumber.ContributionNotification:
                    Log.Warn("Client should not be sending UserNotification Message if following protocol");
                    break;
                case MessageNumber.LoginRequest:
                    var loginRequest = (LoginRequest) message;
                    User newUser = UpdateUserList(loginRequest);
                    NotifyClientsOfNewUser(newUser);
                    break;
                case MessageNumber.UserNotification:
                    Log.Warn("Client should not be sending User Notification Message if following protocol");
                    break;
                case MessageNumber.UserSnapshotRequest:
                    SendUserSnapshot(e.SendersStream);
                    break;
                case MessageNumber.UserSnapshot:
                    Log.Warn("Client should not be sending User Snapshot Message if following protocol");
                    break;
                case MessageNumber.ClientDisconnection:
                    RemoveDisconnectedClient();
                    break;
                default:
                    Log.Warn("Shared classes assembly does not have a definition for message identifier: " + message.Identifier);
                    break;
            }
        }

        private void SendUserSnapshot(NetworkStream sendersStream)
        {
            ISerialiser userSnapshotSerialiser = serialiserFactory.GetSerialiser<UserSnapshot>();
            var userSnapshot = new UserSnapshot(connectedUsers);
            userSnapshotSerialiser.Serialise(userSnapshot, sendersStream);
        }

        private void NotifyClientsOfNewUser(User newUser)
        {
            ISerialiser messageSerialiser = serialiserFactory.GetSerialiser<UserNotification>();

            foreach (TcpClient client in connectedClients)
            {
                NetworkStream clientStream = client.GetStream();
                var userNotification = new UserNotification(newUser, NotificationType.Create);
                messageSerialiser.Serialise(userNotification, clientStream);
            }
        }

        private User UpdateUserList(LoginRequest message)
        {
            var user = new User(message.UserName);
            connectedUsers.Add(user);
            return user;
        }

        private void SendContributionNotificationToClients(ContributionRequest message)
        {
            ISerialiser messageSerialiser = serialiserFactory.GetSerialiser<ContributionNotification>();

            foreach (TcpClient client in connectedClients)
            {
                NetworkStream clientStream = client.GetStream();

                var contributionNotification = new ContributionNotification(message.Contribution);

                messageSerialiser.Serialise(contributionNotification, clientStream);
            }
        }

        private void AddConnectedClient(TcpClient client)
        {
            connectedClients.Add(client);
            Log.Info("Added client to connectedClients list");
        }

        private void RemoveDisconnectedClient()
        {
            TcpClient disconnectedClient = null;

            foreach (TcpClient client in connectedClients.Where(client => !client.Connected))
            {
                disconnectedClient = client;
            }

            if (disconnectedClient != null)
            {
                connectedClients.Remove(disconnectedClient);
                Log.Info("Client successfully removed from ConnectedClients list");
            }
        }
    }
}