using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using SharedClasses;
using SharedClasses.Serialisation;

namespace Server
{
    public class Server
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(typeof (Server));

        private Message clientMessage;

        // Main server socket
        private Socket serverSocket;

        private IPEndPoint ipEndPoint;

        // List of connected clients
        private List<ConnectedClients> connectedClients = new List<ConnectedClients>();

        // Use Strategy pattern to chose what TCP Serialisation method to use
        private ITcpSendBehaviour sendBehaviour;

        private const int PortNumber = 5004;

        public Server(ITcpSendBehaviour sendBehaviour)
        {

            var workerThread = new Thread(StartToListen);
            workerThread.Start();
            SetSerialiseMethod(sendBehaviour);

            //StartTcpListen();
        }

        private void StartToListen()
        {
            // Assign the IP of the machine and listen on a specified port.
            ipEndPoint = new IPEndPoint(IPAddress.Any, PortNumber);

            // Create a TCP socket
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            serverSocket.Bind(ipEndPoint);

            serverSocket.Listen(10);

            while (true)
            {
                Log.Info("Listening for new connection");
                AddToConnectedClients(serverSocket.Accept());
            }
        }

        private void AddToConnectedClients(Socket clientSocket)
        {
            connectedClients.Add(new ConnectedClients(clientSocket));
            Log.Info("New client connected");
        }

        public void SetSerialiseMethod(ITcpSendBehaviour sendBehaviour)
        {
            this.sendBehaviour = sendBehaviour;
            Log.Info("Server's send behaviour set to " + this.sendBehaviour + " method");
        }
    }
}