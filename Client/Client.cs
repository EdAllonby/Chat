using System;
using System.Net.Sockets;
using System.Threading;
using log4net;
using SharedClasses;

namespace Client
{
    internal class Client
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (Client));

        private static NetworkStream stream;

        public Client()
        {
            TcpClient client = ConnectToServer();

            if (client != null)
            {
                while (true)
                {
                    try
                    {
                        string test = Console.ReadLine();
                        var message = new Message(test);

                        message.Serialise(stream);
                    }
                    catch (Exception e)
                    {
                        Log.Error(e);

                        //close the client and stream
                        stream.Close();
                        Log.Info("Stream closed");
                        client.Close();
                        Log.Info("Client connection closed");
                    }
                }
            }
        }

        private static TcpClient ConnectToServer()
        {
            try
            {
                Log.Info("Client looking for server");

                var client = new TcpClient("localhost", 5004);
                Log.Info("Client found server, connection created");

                stream = client.GetStream();
                Log.Info("Created stream with Server");

                var messageListenerThread = new Thread(() => ReceiveMessageListener(stream, client))
                {
                    Name = "MessageListenerThread"
                };
                messageListenerThread.Start();
                return client;
            }
            catch (SocketException socketException)
            {
                Log.Error("Could not connect to the server, exiting...");
                return null;
            }
        }

        private static void ReceiveMessageListener(NetworkStream stream, TcpClient client)
        {
            Log.Info("Message listener thread started");
            bool connection = true;
            while (connection)
            {
                try
                {
                    Message message = Message.Deserialise(stream);
                    Log.Info("Message deserialised. Client sent: " + message.GetMessage());
                    Console.WriteLine("A client sent: " + message.GetMessage());
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
    }
}