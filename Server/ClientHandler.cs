﻿using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using log4net;
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
                case 1: // User Request
                    var contributionRequest = (ContributionRequest) message;
                    SendNotificationToClients(contributionRequest);
                    break;
                case 2: // User Notification
                    Log.Warn("Client should not be sending UserNotification Message if following protocol");
                    break;
                case 3: // Login Request
                    var loginRequest = (LoginRequest) message;
                    User newUser = UpdateUserList(loginRequest);
                    NotifyClientsOfNewUser(newUser);
                    break;
                case 4: // User Notification
                    Log.Warn("Client should not be sending User Notification Message if following protocol");
                    break;
                case 5: // User Snapshot Request
                    NetworkStream sendersStream = e.SendersStream;
                    SendUserSnapshot(sendersStream);
                    break;
                case 6: // User Snapshot
                    Log.Warn("Client should not be sending User Snapshot Message if following protocol");
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

        private void SendNotificationToClients(ContributionRequest message)
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

        private void RemoveDisconnectedClient(TcpClient client)
        {
            connectedClients.Remove(client);
            Log.Info("Client successfully removed from ConnectedClients list");
        }
    }
}