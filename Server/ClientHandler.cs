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

        public static void ReceiveMessageListener(NetworkStream stream, TcpClient client)
        {
            bool connection = true;
            TotalListeners++;
            while (connection)
            {
                try
                {
                    Message message = Message.Deserialise(stream);
                    Log.Info("Message deserialised. Client sent: " + message.GetMessage());

                    SendMessage(message);
                }
                catch (Exception e)
                {
                    Log.Error(e);
                    stream.Close();
                    Log.Info("Client stream closed");
                    client.Close();
                    Log.Info("Client connection closed");
                    connection = false;
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
                Log.Error(e);
            }
        }

        public static void AddConnectedClient(TcpClient client)
        {
            ConnectedClients.Add(new ConnectedClient(client));
            Log.Info("Added client to connectedClients list");
        }

        private static void RemoveDisconnectedClient(ConnectedClient client)
        {
            //TODO: Implement a way of finding out when a client disconnects.
            ConnectedClients.Remove(client);
            Log.Info("Client disconnected" + client.Name);
        }
    }
}