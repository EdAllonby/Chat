using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using log4net;

namespace Server
{
    internal class Server
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (Server));

        private const int PortNumber = 5004;

        public Server()
        {
            Log.Info("Server instance started");
            ListenForNewClients();
        }

        private static void ListenForNewClients()
        {
            var clientListener = new TcpListener(IPAddress.Any, PortNumber);

            clientListener.Start();
            Log.Info("Server started listening for clients to connect");

            while (true)
            {
                TcpClient client = clientListener.AcceptTcpClient();
                
                Log.Info("New client connected");

                var newClient = new ConnectedClient(client);

                ClientHandler.AddConnectedClient(newClient);

                NetworkStream stream = client.GetStream();
                Log.Info("Stream with client established");

                var messageListenerThread = new Thread(() => ClientHandler.ReceiveMessageListener(stream, newClient))
                {
                    Name = "MessageListenerThread" + ClientHandler.TotalListeners
                };
                messageListenerThread.Start();
            }
        }
    }
}