using System.Net.Sockets;
using SharedClasses;

namespace Server
{
    /// <summary>
    /// This class is used to hold the information of clients currently connected to the Server
    /// </summary>
    public class ConnectedClient
    {
        public readonly TcpClient Socket;
        public string Name { get; private set; }
        public Status CurrentStatus { get; private set; }

        public ConnectedClient(TcpClient socket)
        {
            Socket = socket;
            CurrentStatus = Status.Connected;
        }
    }
}