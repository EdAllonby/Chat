using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using log4net;
using SharedClasses;
using SharedClasses.Domain;

namespace Client
{
    internal class Client
    {
        private readonly ILog Log = LogManager.GetLogger(typeof (Client));
        private readonly TcpClient connection;
        private NetworkStream stream;

        private readonly IPAddress targetAddress;
        private readonly int targetPort;

        public Client(IPAddress targetAddress, int targetPort)
        {
            this.targetAddress = targetAddress;
            this.targetPort = targetPort;

            try
            {
                connection = ConnectToServer();

                if (connection != null)
                {
                    while (true)
                    {

                        string test = Console.ReadLine();
                        var message = new Contribution(test);

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
                if (connection != null)
                {
                    connection.Close();
                    Log.Info("Client connection closed");
                }
            }
        }

        private TcpClient ConnectToServer()
        {

            Log.Info("Client looking for server");

            var client = new TcpClient(targetAddress.ToString(), targetPort);
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
            bool serverConnection = true;
            while (serverConnection)
            {
                Contribution contribution = Contribution.Deserialise(stream);

                if (stream.CanRead)
                {
                    Log.Info("Contribution deserialised. Client sent: " + contribution.GetMessage());
                    Console.WriteLine("A client sent: " + contribution.GetMessage());
                }
                else
                {
                    serverConnection = false;
                    Log.Warn("Connection is no longer available, stopping client listener thread");
                }
            }
        }
    }
}