using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using log4net;
using SharedClasses;
using SharedClasses.Protocol;

namespace Server
{
    public class ClientHandler
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (Server));

        private readonly IList<TcpClient> connectedClients = new List<TcpClient>();

        private readonly SerialiserFactory serialiserFactory = new SerialiserFactory();
        private readonly MessageReceiver messageReceiver = new MessageReceiver();


        private int totalListenerThreads = 1;

        public ClientHandler()
        {
            Log.Info("New client handler created");
        }

        public void CreateListenerThreadForClient(TcpClient client)
        {
            AddConnectedClient(client);
            NetworkStream stream = client.GetStream();
            Log.Info("Stream with client established");

            var messageListenerThread = new Thread(() => ReceiveMessageListener(stream, client))
            {
                Name = "MessageListenerThread" + totalListenerThreads
            };

            totalListenerThreads++;

            messageListenerThread.Start();
        }

        private void ReceiveMessageListener(NetworkStream stream, TcpClient client)
        {
            bool connection = true;

            while (connection)
            {
                if (stream.CanRead)
                {
                    messageReceiver.OnNewMessage += NewMessageReceived;
                    messageReceiver.ReceiveMessages(stream);
                }
                else
                {
                    connection = false;
                    Log.Warn("Connection is no longer available, stopping server listener thread for this client");
                    RemoveDisconnectedClient(client);
                }
            }
        }

        private void NewMessageReceived(object sender, MessageEventArgs e)
        {
            IMessage message = e.Message;

            switch (message.Identifier)
            {
                case 1:
                    var contributionRequest = (ContributionRequest) message;
                    SendNotificationToClients(contributionRequest);
                    break;
                case 2:
                    var contributionNotification = (ContributionNotification) message;
                    Log.Info("Server sent: " + contributionNotification.Contribution.GetMessage());
                    break;
                case 3:
                    var loginRequest = (LoginRequest) message;
                    DoSomethingWithLoginRequest(loginRequest);
                    break;
            }
        }

        private static void DoSomethingWithLoginRequest(LoginRequest message)
        {
            Log.Debug("Client sent Login Message Request message");
            Log.Info("Client with username " + message.UserName + " has logged in");
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