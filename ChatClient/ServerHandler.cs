using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using log4net;
using SharedClasses;
using SharedClasses.Message;
using SharedClasses.Serialiser;

namespace ChatClient
{
    /// <summary>
    /// Used to send and receive messages to and from the Server
    /// </summary>
    internal sealed class ServerHandler
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (ServerHandler));

        private readonly SerialiserFactory serialiserFactory = new SerialiserFactory();

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
        public Snapshots ConnectToServer(string username, IPAddress targetAddress, int targetPort)
        {
            TcpClient tcpClient = CreateConnection(targetAddress, targetPort);

            IMessage userRequest = new LoginRequest(username);

            SendConnectionMessage(userRequest, tcpClient);

            LoginResponse loginResponse = GetLoginResponse(tcpClient);

            int clientUserId = loginResponse.User.UserId;

            Snapshots snapshots = GetSnapshots(tcpClient);

            connectionHandler = new ConnectionHandler(clientUserId, tcpClient);

            return snapshots;
        }

        private TcpClient CreateConnection(IPAddress targetAddress, int targetPort)
        {
            const int TimeoutSeconds = 5;

            Log.Info("Client looking for server with address: " + targetAddress + " and port: " + targetPort);

            var serverConnection = new TcpClient();

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

            Log.Info("Client found server, connection created");
            return serverConnection;
        }

        private Snapshots GetSnapshots(TcpClient tcpClient)
        {
            SendConnectionMessage(new UserSnapshotRequest(), tcpClient);

            UserSnapshot userSnapshot = GetUserSnapshot(tcpClient);

            SendConnectionMessage(new ConversationSnapshotRequest(), tcpClient);

            ConversationSnapshot conversationSnapshot = GetConversationSnapshot(tcpClient);

            SendConnectionMessage(new ParticipationSnapshotRequest(), tcpClient);

            ParticipationSnapshot participationSnapshot = GetParticipationSnapshot(tcpClient);

            return new Snapshots(userSnapshot, conversationSnapshot, participationSnapshot);
        }

        private void SendConnectionMessage(IMessage message, TcpClient tcpClient)
        {
            ISerialiser messageSerialiser = serialiserFactory.GetSerialiser(message.Identifier);
            messageSerialiser.Serialise(message, tcpClient.GetStream());
        }

        private LoginResponse GetLoginResponse(TcpClient tcpClient)
        {
            return (LoginResponse) GetIMessage(tcpClient);
        }

        private UserSnapshot GetUserSnapshot(TcpClient tcpClient)
        {
            return (UserSnapshot) GetIMessage(tcpClient);
        }

        private ConversationSnapshot GetConversationSnapshot(TcpClient tcpClient)
        {
            return (ConversationSnapshot) GetIMessage(tcpClient);
        }

        private ParticipationSnapshot GetParticipationSnapshot(TcpClient tcpClient)
        {
            return (ParticipationSnapshot) GetIMessage(tcpClient);
        }

        private IMessage GetIMessage(TcpClient tcpClient)
        {
            var messageIdentifierSerialiser = new MessageIdentifierSerialiser();

            MessageNumber messageIdentifier = messageIdentifierSerialiser.DeserialiseMessageIdentifier(tcpClient.GetStream());

            ISerialiser serialiser = serialiserFactory.GetSerialiser(messageIdentifier);

            return serialiser.Deserialise(tcpClient.GetStream());
        }
    }
}