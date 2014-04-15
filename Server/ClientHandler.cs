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
            var newClient = new ConnectedClient(client, new User("Login message not yet sent for this connected client"));
            AddConnectedClient(newClient);

            var messageListenerThread = new Thread(() => messageReceiver.ReceiveMessages(newClient))
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
                    UpdateConnectClients((LoginRequest) message, e.ConnectedClient);
                    var loginRequest = (LoginRequest) message;
                    var newUser = new User(loginRequest.UserName);
                    NotifyClientsOfNewUser(newUser);
                    UpdateMessageReceiverConnectedClient(e.ConnectedClient.TcpClient, newUser);
                    break;
                case MessageNumber.UserNotification:
                    Log.Warn("Client should not be sending User Notification Message if following protocol");
                    break;
                case MessageNumber.UserSnapshotRequest:
                    SendUserSnapshot(e.ConnectedClient);
                    break;
                case MessageNumber.UserSnapshot:
                    Log.Warn("Client should not be sending User Snapshot Message if following protocol");
                    break;
                case MessageNumber.ClientDisconnection:
                    RemoveConnectedClient(e.ConnectedClient);
                    NotifyClientsOfDisconnectedUser(e.ConnectedClient.User);
                    break;
                default:
                    Log.Warn("Shared classes assembly does not have a definition for message identifier: " + message.Identifier);
                    break;
            }
        }

        private void UpdateMessageReceiverConnectedClient(TcpClient tcpClient, User user)
        {
            messageReceiver.ConnectedClient = new ConnectedClient(tcpClient, user);
            Log.Info("MessageReceiver for this thread has been assigned username " + user.UserName);
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

        private void UpdateConnectClients(LoginRequest loginRequest, ConnectedClient newClient)
        {
            foreach (ConnectedClient client in connectedClients.Where(client => newClient.TcpClient.Equals(client.TcpClient)))
            {
                client.User = new User(loginRequest.UserName);
            }
        }

        private void SendContributionNotificationToClients(ContributionRequest message)
        {
            ISerialiser messageSerialiser = serialiserFactory.GetSerialiser<ContributionNotification>();

            foreach (ConnectedClient client in connectedClients)
            {
                NetworkStream clientStream = client.TcpClient.GetStream();

                var contributionNotification = new ContributionNotification(message.Contribution);

                messageSerialiser.Serialise(contributionNotification, clientStream);
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

            foreach (ConnectedClient connectedClient in connectedClients.Where(connectedClient => connectedClient.User.UserName == client.User.UserName))
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