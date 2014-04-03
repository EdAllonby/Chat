using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using log4net;
using SharedClasses.Protocol;

namespace Server
{
    public class ClientHandler
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (Server));

        private readonly IList<TcpClient> connectedClients = new List<TcpClient>();

        private readonly SerialiserFactory serialiserFactory = new SerialiserFactory();

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
            IMessage clientLoginRequest = GetClientLoginRequest(stream);

            while (connection)
            {
                if (stream.CanRead)
                {
                    ReceiveMessage(stream);
                }
                else
                {
                    connection = false;
                    Log.Warn("Connection is no longer available, stopping server listener thread for this client");
                    RemoveDisconnectedClient(client);
                }
            }
        }

        private IMessage GetClientLoginRequest(NetworkStream stream)
        {
            Log.Debug("Waiting for LoginRequest message type to be sent from client");
            int messageIdentifier = MessageIdentifierSerialiser.DeserialiseMessageIdentifier(stream);

            ISerialiser serialiser = serialiserFactory.GetSerialiser(messageIdentifier);

            LoginRequest loginRequest = (LoginRequest)serialiser.Deserialise(stream);

            Log.Debug("Client sent Login Message Request message");
            Log.Info("Client with username " + loginRequest.UserName + " has logged in");
            return loginRequest;
        }

        private void ReceiveMessage(NetworkStream stream)
        {
            Log.Debug("Waiting for ContributionNotification message type to be sent from client");
            int messageIdentifier = MessageIdentifierSerialiser.DeserialiseMessageIdentifier(stream);

            ISerialiser serialiser = serialiserFactory.GetSerialiser(messageIdentifier);

            IMessage message = serialiser.Deserialise(stream);

            switch (messageIdentifier)
            {
                case 1:
                    ContributionRequest contributionRequest = (ContributionRequest) message;
                    SendNotificationToClients(contributionRequest);
                    Console.WriteLine("The Server sent: " + contributionRequest.Contribution.GetMessage());
                    break;
                case 2:
                    ContributionNotification contributionNotification = (ContributionNotification) message;
                    Log.Info("Server sent: " + contributionNotification.Contribution.GetMessage());
                    Console.WriteLine("The Server sent: " + contributionNotification.Contribution.GetMessage());
                    break;
                case 3:
                    LoginRequest loginRequest = (LoginRequest) message;
                    Log.Info("Server sent: " + loginRequest.UserName);
                    Console.WriteLine("The Server sent: " + loginRequest.UserName);
                    break;
            }
        }

        private void SendNotificationToClients(IMessage message)
        {
            ISerialiser messageSerialiser = serialiserFactory.GetSerialiser<ContributionNotification>();

            foreach (TcpClient client in connectedClients)
            {
                NetworkStream clientStream = client.GetStream();

                ContributionRequest request = (ContributionRequest) message;

                ContributionNotification contributionNotification = new ContributionNotification(request.Contribution);

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