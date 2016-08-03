using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using log4net;
using Server.MessageHandler;
using SharedClasses;
using SharedClasses.Message;

namespace Server
{
    /// <summary>
    /// Handles the logic for incoming <see cref="IMessage" /> passed from ConnectionHandler.
    /// </summary>
    internal sealed class Server
    {
        private const int PortNumber = 5004;
        private static readonly ILog Log = LogManager.GetLogger(typeof(Server));
        private readonly MessageHandlerRegistry messageHandlerRegistry;

        private readonly OnEntityChangedHandler onConversationChangedHandler;
        private readonly OnEntityChangedHandler onParticipationChangedHandler;
        private readonly OnEntityChangedHandler onUserChangedHandler;

        private readonly IServiceRegistry serviceRegistry;

        private bool isServerRunning;

        public Server(IServiceRegistry serviceRegistry)
        {
            this.serviceRegistry = serviceRegistry;

            onUserChangedHandler = new OnUserChangedHandler(serviceRegistry);
            onConversationChangedHandler = new OnConversationChangedHandler(serviceRegistry);
            onParticipationChangedHandler = new OnParticipationChangedHandler(serviceRegistry);

            messageHandlerRegistry = new MessageHandlerRegistry(serviceRegistry);

            isServerRunning = true;
            Log.Info("Server instance started");

            ListenForNewClients();
        }

        public void Shutdown()
        {
            Log.Debug("Starting server shutdown.");
            isServerRunning = false;
            onUserChangedHandler.StopOnMessageChangedHandling();
            onConversationChangedHandler.StopOnMessageChangedHandling();
            onParticipationChangedHandler.StopOnMessageChangedHandling();
            Log.Debug("Server shutdown process finished.");
        }

        private void ListenForNewClients()
        {
            var clientListener = new TcpListener(IPAddress.Any, PortNumber);
            clientListener.Start();
            Log.Info("Server started listening for clients to connect");

            while (isServerRunning)
            {
                TcpClient client = clientListener.AcceptTcpClient();

                Log.Info("New client connection found. Starting login initialisation process.");

                client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);

                InitialiseNewClient(client);
            }
        }

        private void InitialiseNewClient(TcpClient tcpClient)
        {
            var clientHandler = new ClientHandler();

            LoginResponse loginResponse = clientHandler.InitialiseClient(tcpClient, serviceRegistry);

            if (loginResponse.LoginResult == LoginResult.Success)
            {
                clientHandler.MessageReceived += OnMessageReceived;
            }
        }

        private void OnMessageReceived(object sender, MessageEventArgs e)
        {
            IMessage message = e.Message;
            IMessageHandler handler = null;
            try
            {
                handler = messageHandlerRegistry.MessageHandlersIndexedByMessageIdentifier[message.MessageIdentifier];
            }
            catch (KeyNotFoundException keyNotFoundException)
            {
                Log.Error("Server is not supposed to handle message with identifier: " + e.Message.MessageIdentifier, keyNotFoundException);
            }

            handler?.HandleMessage(message);
        }
    }
}