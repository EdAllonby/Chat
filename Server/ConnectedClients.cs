using System.Net.Sockets;

namespace Server
{
    internal class ConnectedClients
    {
        public readonly Socket socket;

        public ConnectedClients(Socket socket)
        {
            this.socket = socket;
        }
    }
}