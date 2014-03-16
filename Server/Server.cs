using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using SharedClasses;

namespace Server
{
    internal class Server
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(typeof (Server));

        public Server()
        {
            ListenForNewClients();   
        }

        private static void ListenForNewClients()
        {
            var clientListener = new TcpListener(IPAddress.Loopback, 5004);
            int listenerCount = 1;

            clientListener.Start();
            Log.Info("Server started listening for clients to connect");

            while (true)
            {
                TcpClient client = clientListener.AcceptTcpClient();
                Log.Info("New client connected");

                NetworkStream stream = client.GetStream();
                Log.Info("Stream with client established");

                var messageListenerThread = new Thread(() => ReceiveMessageListener(stream, client))
                {
                    Name = "MessageListenerThread" + listenerCount
                };
                listenerCount++;
                messageListenerThread.Start();
            }
        }


        private static void ReceiveMessageListener(NetworkStream stream, TcpClient client)
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
                message.Serialise(stream);
            }
            catch (Exception e)
            {
                Log.Error(e);

                //close the client and stream
                stream.Close();
                Log.Info("Stream closed");
            }
        }
    }
}
