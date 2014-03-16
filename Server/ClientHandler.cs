using System;
using System.Collections.Generic;
using System.Net.Sockets;
using SharedClasses;

namespace Server
{
    public static class ClientHandler
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(typeof(Server));

        public static List<ConnectedClients> ConnectedClients = new List<ConnectedClients>();


        public static void ReceiveMessageListener(NetworkStream stream, TcpClient client)
        {
            bool connection = true;
            while (connection)
            {
                try
                {
                    // you have to cast the deserialized object
                    var message = Message.Deserialise(stream);
                    Log.Info("Message deserialised. Client sent: " + message.Text + " at: " + message.MessageTimeStamp);

                    SendMessage(stream, message);
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

        private static void SendMessage(NetworkStream stream, Message message)
        {
            try
            {
                Log.Info("Attempt to serialise message and send to the client");

                foreach (var client in ConnectedClients)
                {
                    NetworkStream clientStream = client.socket.GetStream();
                    message.Serialise(clientStream);
                }
            }
            catch (Exception e)
            {
                Log.Error(e);

                //close the client and stream
                stream.Close();
                Log.Info("Stream closed");
            }
        }

        public static void AddConnectedClient(TcpClient client)
        {
            ConnectedClients.Add(new ConnectedClients(client));
            Log.Info("Added client to connectedClients list");
        }
    }
}
