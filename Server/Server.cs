using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Server
{
    internal class Server
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(typeof (Server));

        private const int PortNumber = 5004;

        public Server()
        {
            ListenForNewClients();
        }

        private static void ListenForNewClients()
        {
            var clientListener = new TcpListener(IPAddress.Loopback, PortNumber);
            int listenerCount = 1;

            clientListener.Start();
            Log.Info("Server started listening for clients to connect");

            while (true)
            {
                TcpClient client = clientListener.AcceptTcpClient();
                Log.Info("New client connected");

                ClientHandler.AddConnectedClient(client);

                NetworkStream stream = client.GetStream();
                Log.Info("Stream with client established");


                var messageListenerThread = new Thread(() => ClientHandler.ReceiveMessageListener(stream, client))
                {
                    Name = "MessageListenerThread" + listenerCount
                };
                messageListenerThread.Start();
                listenerCount++;
            }
        }
    }
}