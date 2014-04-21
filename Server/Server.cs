using System.Net;
using System.Net.Sockets;
using log4net;

namespace Server
{
    internal sealed class Server
    {
        private const int PortNumber = 5004;
        private static readonly ILog Log = LogManager.GetLogger(typeof (Server));

        private readonly ClientHandler clientHandler = new ClientHandler();
        private readonly TcpListener clientListener = new TcpListener(IPAddress.Any, PortNumber);

        public Server()
        {
            Log.Info("Server instance started");
            ListenForNewClients();
        }

        private void ListenForNewClients()
        {
            clientListener.Start();
            Log.Info("Server started listening for clients to connect");

            while (true)
            {
                TcpClient client = clientListener.AcceptTcpClient();
                Log.Info("New client connected");
                
                client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);

                clientHandler.CreateListenerThreadForClient(client);
            }
        }
    }
}