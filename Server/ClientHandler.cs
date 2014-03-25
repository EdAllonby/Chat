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

        private readonly ContributionNotificationSerialiser contributionNotificationSerialiser = new ContributionNotificationSerialiser();
        private readonly ContributionRequestSerialiser contributionRequestSerialiser = new ContributionRequestSerialiser();
        private readonly LoginRequestSerialiser loginRequestSerialiser = new LoginRequestSerialiser();
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
            LoginRequest clientLoginRequest = GetClientLoginRequest(stream);

            while (connection)
            {

                ContributionRequest contributionRequest = contributionRequestSerialiser.Deserialise(stream);

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

        private LoginRequest GetClientLoginRequest(NetworkStream stream)
        {
            Log.Debug("Waiting for LoginRequest message to be sent from client");
            LoginRequest loginRequest = loginRequestSerialiser.Deserialise(stream);
            Log.Info("Client with username " + loginRequest.UserName + " has logged in");

            return loginRequest;
        }

        private void SendMessage(ContributionNotification contribution)
        {
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