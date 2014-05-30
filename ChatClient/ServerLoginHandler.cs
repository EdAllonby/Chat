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
    /// Creates a connection to the Server and gets the entity snapshots the client needs.
    /// </summary>
    internal sealed class ServerLoginHandler
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (ServerLoginHandler));

        private readonly SerialiserFactory serialiserFactory = new SerialiserFactory();

        private readonly RepositoryManager repositoryManager;

        private TcpClient loginConnection;

        public ServerLoginHandler(RepositoryManager repositoryManager)
        {
            this.repositoryManager = repositoryManager;
        }

        public ConnectionHandler CreateServerConnectionHandler(int clientUserId)
        {
            return new ConnectionHandler(clientUserId, loginConnection);
        }

        public LoginResponse ConnectToServer(LoginDetails loginDetails)
        {
            CreateConnection(loginDetails.Address, loginDetails.Port);

            IMessage userRequest = new LoginRequest(loginDetails.Username);
            SendConnectionMessage(userRequest, loginConnection);
            LoginResponse loginResponse = (LoginResponse)GetConnectionIMessage(loginConnection);
            return loginResponse;
        }

        public void GetSnapshots(int userId)
        {
            SendConnectionMessage(new UserSnapshotRequest(userId), loginConnection);

            UserSnapshot userSnapshot = (UserSnapshot)GetConnectionIMessage(loginConnection);

            SendConnectionMessage(new ConversationSnapshotRequest(userId), loginConnection);

            ConversationSnapshot conversationSnapshot = (ConversationSnapshot)GetConnectionIMessage(loginConnection);

            SendConnectionMessage(new ParticipationSnapshotRequest(userId), loginConnection);

            ParticipationSnapshot participationSnapshot = (ParticipationSnapshot)GetConnectionIMessage(loginConnection);

            repositoryManager.UserRepository.AddUsers(userSnapshot.Users);
            repositoryManager.ConversationRepository.AddConversations(conversationSnapshot.Conversations);
            repositoryManager.ParticipationRepository.AddParticipations(participationSnapshot.Participations);
        }

        private void CreateConnection(IPAddress targetAddress, int targetPort)
        {
            const int TimeoutSeconds = 5;

            Log.Info("ClientService looking for server with address: " + targetAddress + ":" + targetPort);

            var connection = new TcpClient();

            connection.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);

            IAsyncResult asyncResult = connection.BeginConnect(targetAddress.ToString(), targetPort, null, null);
            WaitHandle waitHandle = asyncResult.AsyncWaitHandle;
            try
            {
                if (!asyncResult.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(TimeoutSeconds), false))
                {
                    connection.Close();
                    throw new TimeoutException();
                }

                connection.EndConnect(asyncResult);
            }
            finally
            {
                waitHandle.Close();
            }

            Log.Info("ClientService found server, connection created");
            loginConnection = connection;
        }

        private void SendConnectionMessage(IMessage message, TcpClient tcpClient)
        {
            ISerialiser messageSerialiser = serialiserFactory.GetSerialiser(message.MessageIdentifier);
            messageSerialiser.Serialise(message, tcpClient.GetStream());
        }

        private IMessage GetConnectionIMessage(TcpClient tcpClient)
        {
            var messageIdentifierSerialiser = new MessageIdentifierSerialiser();

            MessageIdentifier messageIdentifier = messageIdentifierSerialiser.DeserialiseMessageIdentifier(tcpClient.GetStream());

            ISerialiser serialiser = serialiserFactory.GetSerialiser(messageIdentifier);

            return serialiser.Deserialise(tcpClient.GetStream());
        }
    }
}