using System;
using System.Net.Sockets;
using SharedClasses;
using SharedClasses.Domain;
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

        /// <summary>
        /// Fires when a message has been sent from the client.
        /// </summary>
        public event EventHandler<MessageEventArgs> MessageReceived;

        /// <summary>
        /// Logs in a requested Client to the Server.
        /// </summary>
        /// <param name="tcpClient">The client's connection.</param>
        /// <param name="userRepository">A generator for assigning the client a unique user ID.</param>
        /// <param name="entityIdAllocator">The server's list of repositories used to give the client necessary entity collections.</param>
        /// <returns>A login response <see cref="IMessage"/> with the details of the login attempt.</returns>
        public LoginResponse InitialiseClient(TcpClient tcpClient, UserRepository userRepository, EntityIdAllocatorFactory entityIdAllocator)
        {
            clientLoginHandler = new ClientLoginHandler(userRepository);
            LoginResponse loginResponse = clientLoginHandler.InitialiseNewClient(tcpClient, entityIdAllocator);

            if (loginResponse.LoginResult == LoginResult.Success)
            {
                CreateConnectionHandler(loginResponse.User.Id, tcpClient);
            }

            return loginResponse;
        }

        /// <summary>
        /// Creates a new <see cref="ConnectionHandler"/> to connect the client and the server.
        /// </summary>
        /// <param name="userId">The user id to link this connection handler with.</param>
        /// <param name="tcpClient">The TCP connection between this client and the Server.</param>
        private void CreateConnectionHandler(int userId, TcpClient tcpClient)
        {
            connectionHandler = new ConnectionHandler(userId, tcpClient);
            connectionHandler.MessageReceived += OnMessageReceived;
        }

        /// <summary>
        /// Send an <see cref="IMessage"/> to the client.
        /// </summary>
        /// <param name="message">The <see cref="IMessage"/> to send to the client.</param>
        public void SendMessage(IMessage message)
        {
            connectionHandler.SendMessage(message);
        }

        private void OnMessageReceived(object sender, MessageEventArgs e)
        {
            EventHandler<MessageEventArgs> messageReceivedCopy = MessageReceived;

            if (messageReceivedCopy != null)
            {
                MessageReceived(sender, e);
            }
        }

        public void Dispose()
        {
            connectionHandler.Dispose();
        }
    }
}