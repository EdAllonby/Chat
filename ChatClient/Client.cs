using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using log4net;
using SharedClasses;
using SharedClasses.Domain;
using SharedClasses.Protocol;

namespace ChatClient
{
    internal class Client
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (Client));
        private static Client uniqueClientInstance;

        private readonly MessageReceiver messageReceiver = new MessageReceiver();
        private readonly SerialiserFactory serialiserFactory = new SerialiserFactory();

        private readonly IPAddress targetAddress;
        private readonly int targetPort;
        private readonly string userName;
        private IList<User> connectedUsers = new List<User>();
        private NetworkStream stream;

        private Client(string userName, IPAddress targetAddress, int targetPort)
        {
            this.userName = userName;
            this.targetAddress = targetAddress;
            this.targetPort = targetPort;
            ConnectToServer();

            while (true)
            {
                SendContributionRequestMessage();
            }
        }

        public static Client GetInstance(string username, IPAddress targetAddress, int targetPort)
        {
            return uniqueClientInstance ?? (uniqueClientInstance = new Client(username, targetAddress, targetPort));
        }

        public static Client GetInstance()
        {
            return uniqueClientInstance ?? (uniqueClientInstance = new Client("unassigned", new IPAddress(127000000000), 5004));
        }

        private void ConnectToServer()
        {
            Log.Info("Client looking for server");
            var client = new TcpClient(targetAddress.ToString(), targetPort);
            Log.Info("Client found server, connection created");

            stream = client.GetStream();
            Log.Info("Created stream with Server");

            var messageListenerThread = new Thread(ReceiveMessageListener)
            {
                Name = "MessageListenerThread"
            };
            messageListenerThread.Start();

            SendLoginRequest();
            SendUserSnaphotRequest();
        }

        private void SendLoginRequest()
        {
            ISerialiser loginRequestSerialiser = serialiserFactory.GetSerialiser<LoginRequest>();
            var loginRequest = new LoginRequest(userName);
            loginRequestSerialiser.Serialise(loginRequest, stream);
        }

        private void SendUserSnaphotRequest()
        {
            ISerialiser userSnapshotRequestSerialiser = serialiserFactory.GetSerialiser<UserSnapshotRequest>();
            var userSnapshotRequest = new UserSnapshotRequest();
            userSnapshotRequestSerialiser.Serialise(userSnapshotRequest, stream);
        }

        private void SendContributionRequestMessage()
        {
            string message = Console.ReadLine();
            var clientContribution = new ContributionRequest(new Contribution(message));
            ISerialiser serialiser = serialiserFactory.GetSerialiser<ContributionRequest>();
            serialiser.Serialise(clientContribution, stream);
        }

        private void ReceiveMessageListener()
        {
            Log.Info("Message listener thread started");
            messageReceiver.OnNewMessage += NewMessageReceived;
            messageReceiver.ReceiveMessages(stream);
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
                    var contributionNotification = (ContributionNotification) message;
                    Log.Info("Server sent: " + contributionNotification.Contribution.GetMessage());
                    Console.WriteLine("The Server sent: " + contributionNotification.Contribution.GetMessage());
                    break;
                case MessageNumber.LoginRequest: //Login Request
                    Log.Warn("Server shouldn't be sending a LoginRequest message to a client if following protocol");
                    break;
                case MessageNumber.UserNotification: //User Notification
                    var userNotification = (UserNotification) message;
                    NotifyClientOfNewUser(userNotification);
                    break;
                case MessageNumber.UserSnapshotRequest: //User Snapshot Request
                    Log.Warn("Server shouldn't be sending a User Snapshot Request message to a client if following protocol");
                    break;
                case MessageNumber.UserSnapshot: //User Snapshot
                    var userSnapshot = (UserSnapshot) message;
                    ListCurrentUsers(userSnapshot);
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
        }

        private void NotifyClientOfNewUser(UserNotification userNotification)
        {
            if (userNotification.Notification == NotificationType.Create)
            {
                connectedUsers.Add(userNotification.User);

                Log.Info("New user logged in successfully, currently connected users: ");

                foreach (User user in connectedUsers)
                {
                    Log.Info("User: " + user.UserName);
                }
            }
            else if (userNotification.Notification == NotificationType.Delete)
            {
                connectedUsers.Remove(userNotification.User);
                Log.Info("User " + userNotification.User + " logged out. Removing from connectedClients list");
            }
        }
    }
}