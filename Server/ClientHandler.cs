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

        public static List<ConnectedClients> ConnectedClients = new List<ConnectedClients>();


        public static void ReceiveMessageListener(NetworkStream stream, TcpClient client)
        {
            bool connection = true;
            while (connection)
            {
                try
                {
                    // you have to cast the deserialized object
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
                foreach (ConnectedClients client in ConnectedClients)
                {
                    NetworkStream clientStream = client.socket.GetStream();
                    message.Serialise(clientStream);
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

        public static void AddConnectedClient(TcpClient client)
        {
            ConnectedClients.Add(new ConnectedClients(client));
            Log.Info("Added client to connectedClients list");
        }

        public static void RemoveDisconnectedClient(TcpClient client)
        {
            //TODO: Implement a way of finding out when a client disconnects.
            ConnectedClients.Remove(new ConnectedClients(client));
        }
    }
}