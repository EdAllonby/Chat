using System;
using System.Net;
using log4net;
using SharedClasses;
using SharedClasses.Message;

namespace ChatClient
{
    /// <summary>
    /// Used to send and receive messages to and from the Server
    /// </summary>
    internal sealed class ServerHandler
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (ServerHandler));

        private ConnectionHandler connectionHandler;

        /// <summary>
        /// This event will hold the message when a new <see cref="IMessage"/> is sent to the Client from the Server
        /// </summary>
        public event EventHandler<MessageEventArgs> OnNewMessage
        {
            add { connectionHandler.OnNewMessage += value; }
            remove { connectionHandler.OnNewMessage -= value; }
        }

        /// <summary>
        /// Sends an <see cref="IMessage"/> to the Server.
        /// </summary>
        /// <param name="message"></param>
        public void SendMessage(IMessage message)
        {
            connectionHandler.SendMessage(message);
        }

        /// <summary>
        /// Connects the client to the server with the credentials
        /// </summary>
        /// <param name="username">The name the user wants to have.</param>
        /// <param name="targetAddress">The address of the Server.</param>
        /// <param name="targetPort">The port the Server is running on.</param>
        /// <returns>The currently connected users on the Server.</returns>
        public InitialisedData ConnectToServer(string username, IPAddress targetAddress, int targetPort)
        {
            ServerLoginHandler loginHandler = new ServerLoginHandler();

            InitialisedData initialisedData = loginHandler.ConnectToServer(username, targetAddress, targetPort);

            connectionHandler = loginHandler.CreateServerConnectionHandler();

            Log.DebugFormat("Connection process to the server has finished");

            return initialisedData;
        }
    }
}