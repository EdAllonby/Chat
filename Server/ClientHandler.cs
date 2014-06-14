using System;
using System.Net.Sockets;
using SharedClasses;
using SharedClasses.Message;

namespace Server
{
    /// <summary>
    /// Handles message handling for a unique client in the system
    /// </summary>
    internal sealed class ClientHandler : IDisposable
    {
        private ClientLoginHandler clientLoginHandler;
        private ConnectionHandler connectionHandler;

        public void Dispose()
        {
            connectionHandler.Dispose();
        }

        public event EventHandler<MessageEventArgs> MessageReceived;

        /// <summary>
        /// Logs in a requested Client to the Server.
        /// </summary>
        /// <param name="tcpClient">The client's connection.</param>
        /// <param name="entityGeneratorFactory">A generator for assigning the client a unique user ID.</param>
        /// <param name="repositoryManager">The server's list of repositories used to give the client necessary entity collections.</param>
        /// <returns></returns>
        public LoginResponse LoginClient(TcpClient tcpClient, RepositoryManager repositoryManager, EntityGeneratorFactory entityGenerator)
        {
            clientLoginHandler = new ClientLoginHandler(repositoryManager);
            return clientLoginHandler.InitialiseNewClient(tcpClient, entityGenerator);
        }

        /// <summary>
        /// Creates a new <see cref="ConnectionHandler"/> to connect the client and the server.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="tcpClient"></param>
        public void CreateConnectionHandler(int userId, TcpClient tcpClient)
        {
            connectionHandler = new ConnectionHandler(userId, tcpClient);
            connectionHandler.MessageReceived += OnConnectionHandlerNewMessageReceived;
        }

        public void SendMessage(IMessage message)
        {
            connectionHandler.SendMessage(message);
        }

        private void OnConnectionHandlerNewMessageReceived(object sender, MessageEventArgs e)
        {
            MessageReceived(sender, e);
        }
    }
}