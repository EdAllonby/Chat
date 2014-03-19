using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using log4net;
using SharedClasses;

namespace Client
{
    internal class Client
    {
        private readonly ILog Log = LogManager.GetLogger(typeof (Client));
        private readonly TcpClient client;
        private NetworkStream stream;
        private IPAddress targetAddress;

        private int port;

        public Client(IPAddress targetAddress, int port)
        {
            this.targetAddress = targetAddress;
            this.port = port;

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
                if (stream != null)
                {
                    stream.Close();
                    Log.Info("Stream closed");
                }
                if (client != null)
                {
                    client.Close();
                    Log.Info("Client connection closed");
                }
            }
        }

        private TcpClient ConnectToServer()
        {

            Log.Info("Client looking for server");

            var client = new TcpClient(targetAddress.ToString(), port);
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

        private void ReceiveMessageListener()
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