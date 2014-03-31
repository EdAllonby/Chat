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

        private int totalListeners = 1;

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
                Name = "MessageListenerThread" + totalListeners
            };

            totalListeners++;

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
                    IMessage contributionNotification = ReceiveContributionRequest(stream);
                    SendMessage(contributionNotification);
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
            int messageIdentifier = MessageType.Deserialise(stream);

            ISerialiser serialiser = serialiserFactory.GetSerialiser(messageIdentifier);

            IMessage loginRequest = serialiser.Deserialise(stream);

            Log.Debug("Client sent Login Message Request message");
            Log.Info("Client with username " + loginRequest.GetMessage() + " has logged in");
            return loginRequest;
        }

        private IMessage ReceiveContributionRequest(NetworkStream stream)
        {
            Log.Debug("Waiting for ContributionNotification message type to be sent from client");
            int messageType = MessageType.Deserialise(stream);

            ISerialiser serialiser = serialiserFactory.GetSerialiser(messageType);

            var contributionRequest = serialiser.Deserialise(stream) as ContributionRequest;

            Log.Debug("Client sent Contribution Request message");

            if (contributionRequest != null)
            {
                Log.Info("Client sent: " + contributionRequest.GetMessage());
                var contributionNotification = new ContributionNotification(contributionRequest.Contribution);
                return contributionNotification;
            }

            return null;
        }

        private void SendMessage(IMessage contribution)
        {
            ISerialiser contributionNotificationSerialiser = new ContributionNotificationSerialiser();

            foreach (TcpClient client in connectedClients)
            {
                NetworkStream clientStream = client.GetStream();

                contributionNotificationSerialiser.Serialise(contribution, clientStream);
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