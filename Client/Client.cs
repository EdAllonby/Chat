using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using log4net;
using SharedClasses;

namespace Client
{
    internal class Client
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (Client));
        private readonly TcpClient client;
        private static NetworkStream stream;

        public Client()
        {
            try
            {
                client = ConnectToServer();

                if (client != null)
                {
                    while (true)
                    {

                        string test = Console.ReadLine();
                        var message = new Message(test);

                        message.Serialise(stream);
                    }
                }
            }
            catch (SocketException socketException)
            {
                Log.Error("Could not connect to the server, exiting...", socketException);
            }
            catch (IOException ioException)
            {
                Log.Error("could not send data to the server, connection lost.", ioException);
            }
            finally
            {
                //close the client and stream
                stream.Close();
                Log.Info("Stream closed");
                client.Close();
                Log.Info("Client connection closed");
            }
        }

        private static
            TcpClient ConnectToServer()
        {

            Log.Info("Client looking for server");

            var client = new TcpClient("localhost", 5004);
            Log.Info("Client found server, connection created");

            stream = client.GetStream();
            Log.Info("Created stream with Server");

            var messageListenerThread = new Thread(ReceiveMessageListener)
            {
                Name = "MessageListenerThread"
            };
            messageListenerThread.Start();
            return client;

        }

        private static void ReceiveMessageListener()
        {
            Log.Info("Message listener thread started");
            bool connection = true;
            while (connection)
            {
                Message message = Message.Deserialise(stream);

                if (stream.CanRead)
                {
                    Log.Info("Message deserialised. Client sent: " + message.GetMessage());
                    Console.WriteLine("A client sent: " + message.GetMessage());
                }
                else
                {
                    connection = false;
                    Log.Warn("Connection is no longer available, stopping client listener thread");
                }
            }
        }
    }
}