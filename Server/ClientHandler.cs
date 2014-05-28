using System.Net.Sockets;
using SharedClasses;
using SharedClasses.Message;

namespace Server
{
    internal class ClientHandler
    {
        private ClientLoginHandler clientLoginHandler;

        public LoginResponse LoginClient(TcpClient tcpClient, EntityGeneratorFactory entityGeneratorFactory, RepositoryManager repositoryManager)
        {
            clientLoginHandler = new ClientLoginHandler(entityGeneratorFactory, repositoryManager);
            return clientLoginHandler.InitialiseNewClient(tcpClient);
        }
        public void CreateConnectionHandler(int userId, TcpClient tcpClient)
        {
            ConnectionHandler = new ConnectionHandler(userId, tcpClient);
        }

        public void SendMessage(IMessage message)
        {
            ConnectionHandler.SendMessage(message);
        }

        public ConnectionHandler ConnectionHandler { get; private set; }
    }
}