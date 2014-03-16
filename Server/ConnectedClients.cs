using System.Net.Sockets;

namespace Server
{
    public class ConnectedClients
    {
        public readonly TcpClient socket;

        public ConnectedClients(TcpClient socket)
        {
            this.socket = socket;
        }
    }
}