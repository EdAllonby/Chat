using System.Net.Sockets;

namespace Server
{
    internal class ConnectedClients
    {
        public readonly TcpClient socket;

        public ConnectedClients(TcpClient socket)
        {
            this.socket = socket;
        }
    }
}