using System;
using System.Collections.Generic;
using System.Net.Sockets;
using log4net;
using SharedClasses;

namespace Server
{
    public static class ClientHandler
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (Server));

        private static readonly IList<ConnectedClient> ConnectedClients = new List<ConnectedClient>();

        public static int TotalListeners { get; private set; }

        public static void ReceiveMessageListener(NetworkStream stream, ConnectedClient client)
        {
            bool connection = true;
            TotalListeners++;
            while (connection)
            {
                Message message = Message.Deserialise(stream);

                if (stream.CanRead)
                {
                    Log.Info("Message deserialised. Client sent: " + message.GetMessage());
                    SendMessage(message);
                }
                else
                {
                    connection = false;
                    Log.Warn("Connection is no longer available, stopping server listener thread for this client");
                    RemoveDisconnectedClient(client);
                }
            }
        }

        private static void SendMessage(Message message)
        {
            try
            {
                foreach (ConnectedClient client in ConnectedClients)
                {
                    if (client.CurrentStatus == Status.Connected)
                    {
                        NetworkStream clientStream = client.Socket.GetStream();
                        message.Serialise(clientStream);
                    }
                    if (client.CurrentStatus == Status.Disconnected)
                    {
                        RemoveDisconnectedClient(client);
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error("Most probably this exception is because client disconnection is not implemented yet " +
                          "and the server thinks a disconnected client is still available to send a message to.", e);
            }
        }

        public static void AddConnectedClient(ConnectedClient client)
        {
            ConnectedClients.Add(client);
            Log.Info("Added client to connectedClients list");
        }

        private static void RemoveDisconnectedClient(ConnectedClient client)
        {
            ConnectedClients.Remove(client);
            Log.Info("Client successfully removed from ConnectedClients list");
        }
    }
}