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

        private void ListenForNewClients()
        {
            var clientListener = new TcpListener(IPAddress.Any, PortNumber);

            var clientHandler = new ClientHandler();

            clientListener.Start();
            Log.Info("Server started listening for clients to connect");

            while (true)
            {
                TcpClient client = clientListener.AcceptTcpClient();
                Log.Info("New client connected");

                clientHandler.CreateListenerThreadForClient(client);
            }
        }
    }
}