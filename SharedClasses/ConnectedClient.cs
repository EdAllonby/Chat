using System.Net.Sockets;
using SharedClasses.Domain;

namespace SharedClasses
{
    public sealed class ConnectedClient
    {
        public ConnectedClient(TcpClient tcpClient, User user)
        {
            TcpClient = tcpClient;
            User = user;
        }

        public TcpClient TcpClient { get; private set; }
        public User User { get; set; }
    }
}