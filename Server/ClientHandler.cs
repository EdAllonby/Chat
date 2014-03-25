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

        private readonly ContributionNotificationSerialiser notificationSerialiser = new ContributionNotificationSerialiser();
        private readonly ContributionRequestSerialiser requestSerialiser = new ContributionRequestSerialiser();
        private int totalListeners;

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

            messageListenerThread.Start();
        }

        private void ReceiveMessageListener(NetworkStream stream, TcpClient client)
        {
            bool connection = true;
            totalListeners++;
            while (connection)
            {
                ContributionRequest contributionRequest = requestSerialiser.Deserialise(stream);

                if (stream.CanRead)
                {
                    Log.Info("Client sent: " + contributionRequest.Contribution.GetMessage());
                    var contributionNotification = new ContributionNotification {Contribution = contributionRequest.Contribution};
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

        private void SendMessage(ContributionNotification contribution)
        {
            foreach (TcpClient client in connectedClients)
            {
                NetworkStream clientStream = client.GetStream();

                notificationSerialiser.Serialise(contribution, clientStream);
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