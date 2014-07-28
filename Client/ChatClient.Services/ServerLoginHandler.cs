using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using ChatClient.Services.MessageHandler;
using log4net;
using SharedClasses;
using SharedClasses.Message;
using SharedClasses.Serialiser;

namespace ChatClient.Services
{
    /// <summary>
    /// Creates a connection to the Server.
    /// </summary>
    internal sealed class ServerLoginHandler
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (ServerLoginHandler));

        private readonly SerialiserFactory serialiserFactory = new SerialiserFactory();
        private readonly TcpClient serverConnection = new TcpClient();

        private bool hasUserSnapshotBeenSent;
        private bool hasParticipationSnapshotBeenSent;
        private bool hasConversationSnapshotBeenSent;

        public event EventHandler BootstrapCompleted;

        public ServerLoginHandler()
        {
            UserSnapshotHandler userSnapshotHandler = (UserSnapshotHandler) MessageHandlerRegistry.MessageHandlersIndexedByMessageIdentifier[MessageIdentifier.UserSnapshot];
            ParticipationSnapshotHandler participationSnapshotHandler = (ParticipationSnapshotHandler) MessageHandlerRegistry.MessageHandlersIndexedByMessageIdentifier[MessageIdentifier.ParticipationSnapshot];
            ConversationSnapshotHandler conversationSnapshotHandler = (ConversationSnapshotHandler) MessageHandlerRegistry.MessageHandlersIndexedByMessageIdentifier[MessageIdentifier.ConversationSnapshot];

            userSnapshotHandler.UserBootstrapCompleted += OnUserBootstrapCompleted;
            participationSnapshotHandler.ParticipationBootstrapCompleted += OnParticipationBootstrapCompleted;
            conversationSnapshotHandler.ConversationBootstrapCompleted += OnConversationBootstrapCompleted;
        }

        void OnUserBootstrapCompleted(object sender, EventArgs e)
        {
            hasUserSnapshotBeenSent = true;
            TrySendBootstrapCompleteEvent();
        }

        private void OnParticipationBootstrapCompleted(object sender, EventArgs e)
        {
            hasParticipationSnapshotBeenSent = true;
            TrySendBootstrapCompleteEvent();
        }

        private void OnConversationBootstrapCompleted(object sender, EventArgs e)
        {
            hasConversationSnapshotBeenSent = true;
            TrySendBootstrapCompleteEvent();
        }

        private void TrySendBootstrapCompleteEvent()
        {
            if (hasUserSnapshotBeenSent && hasParticipationSnapshotBeenSent && hasConversationSnapshotBeenSent)
            {
                Log.Debug("Client bootstrap complete. Sending Bootstrap Completed event.");
                OnBootstrapCompleted();
            }
        }
        
        public LoginResponse ConnectToServer(LoginDetails loginDetails, out ConnectionHandler connectionHandler)
        {
            CreateConnection(loginDetails.Address, loginDetails.Port);

            IMessage userRequest = new LoginRequest(loginDetails.Username);
            SendConnectionMessage(userRequest);
            var loginResponse = (LoginResponse) GetConnectionIMessage();

            if (loginResponse.LoginResult == LoginResult.Success)
            {
                BootstrapRepositories(loginResponse.User.Id);

                connectionHandler = new ConnectionHandler(loginResponse.User.Id, serverConnection);

                Log.DebugFormat("Connection process to the server has finished");
            }
            else
            {
                connectionHandler = null;
            }

            return loginResponse;
        }

        private void BootstrapRepositories(int userId)
        {
            SendConnectionMessage(new UserSnapshotRequest(userId));

            SendConnectionMessage(new ConversationSnapshotRequest(userId));

            SendConnectionMessage(new ParticipationSnapshotRequest(userId));
        }

        private void CreateConnection(IPAddress targetAddress, int targetPort)
        {
            const int TimeoutSeconds = 5;

            Log.Info("ClientService looking for server with address: " + targetAddress + ":" + targetPort);

            serverConnection.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);

            IAsyncResult asyncResult = serverConnection.BeginConnect(targetAddress.ToString(), targetPort, null, null);
            WaitHandle waitHandle = asyncResult.AsyncWaitHandle;
            try
            {
                if (!asyncResult.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(TimeoutSeconds), false))
                {
                    serverConnection.Close();
                    throw new TimeoutException();
                }

                serverConnection.EndConnect(asyncResult);
            }
            finally
            {
                waitHandle.Close();
            }

            Log.Info("ClientService found server, connection created");
        }

        private void SendConnectionMessage(IMessage message)
        {
            ISerialiser messageSerialiser = serialiserFactory.GetSerialiser(message.MessageIdentifier);
            messageSerialiser.Serialise(serverConnection.GetStream(), message);
        }

        private IMessage GetConnectionIMessage()
        {
            var messageIdentifierSerialiser = new MessageIdentifierSerialiser();

            MessageIdentifier messageIdentifier = messageIdentifierSerialiser.DeserialiseMessageIdentifier(serverConnection.GetStream());

            ISerialiser serialiser = serialiserFactory.GetSerialiser(messageIdentifier);

            return serialiser.Deserialise(serverConnection.GetStream());
        }

        private void OnBootstrapCompleted()
        {
            EventHandler handler = BootstrapCompleted;
            if (handler != null) handler(this, EventArgs.Empty);
        }
    }
}