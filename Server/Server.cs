using System.Net;
using System.Net.Sockets;
using log4net;

namespace Server
{
    internal class Server
    {
        private const int PortNumber = 5004;
        private static readonly ILog Log = LogManager.GetLogger(typeof (Server));

        private readonly ClientHandler clientHandler = new ClientHandler();
        private TcpListener clientListener;

        public Server()
        {
            Log.Info("Server instance started");
            ListenForNewClients();
        }

        private void ListenForNewClients()
        {
            clientListener = new TcpListener(IPAddress.Any, PortNumber);

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