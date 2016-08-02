using System;
using System.Net.Sockets;
using log4net;
using SharedClasses;
using SharedClasses.Message;

namespace Server
{
    /// <summary>
    /// Handles message handling for a unique client in the system
    /// </summary>
    internal sealed class ClientHandler : IClientHandler, IDisposable
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(ClientHandler));

        private ConnectionHandler connectionHandler;

        /// <summary>
        /// Fires when a message has been sent from the client.
        /// </summary>
        public event EventHandler<MessageEventArgs> MessageReceived;

        /// <summary>
        /// Logs in a requested Client to the Server.
        /// </summary>
        /// <param name="tcpClient">The client's connection.</param>
        /// <param name="serviceRegistry">Holds services to initialise client</param>
        /// <returns>A login response <see cref="IMessage" /> with the details of the login attempt.</returns>
        public LoginResponse InitialiseClient(TcpClient tcpClient, IServiceRegistry serviceRegistry)
        {
            LoginResponse loginResponse = ClientLoginHandler.InitialiseNewClient(tcpClient, serviceRegistry);
            var clientManager = serviceRegistry.GetService<IClientManager>();

            if (loginResponse.LoginResult == LoginResult.Success)
            {
                CreateConnectionHandler(loginResponse.User.Id, tcpClient);

                clientManager.AddClientHandler(loginResponse.User.Id, this);

                Log.InfoFormat($"Client with User Id {loginResponse.User.Id} has successfully logged in.");
            }

            return loginResponse;
        }

        /// <summary>
        /// Send an <see cref="IMessage" /> to the client.
        /// </summary>
        /// <param name="message">The <see cref="IMessage" /> to send to the client.</param>
        public void SendMessage(IMessage message)
        {
            connectionHandler.SendMessage(message);
        }

        public void Dispose()
        {
            connectionHandler.Dispose();
        }

        /// <summary>
        /// Creates a new <see cref="ConnectionHandler" /> to connect the client and the server.
        /// </summary>
        /// <param name="userId">The user id to link this connection handler with.</param>
        /// <param name="tcpClient">The TCP connection between this client and the Server.</param>
        private void CreateConnectionHandler(int userId, TcpClient tcpClient)
        {
            connectionHandler = new ConnectionHandler(userId, tcpClient);
            connectionHandler.MessageReceived += OnMessageReceived;
        }

        private void OnMessageReceived(object sender, MessageEventArgs e)
        {
            EventHandler<MessageEventArgs> messageReceivedCopy = MessageReceived;

            if (messageReceivedCopy != null)
            {
                MessageReceived(sender, e);
            }
        }
    }
}