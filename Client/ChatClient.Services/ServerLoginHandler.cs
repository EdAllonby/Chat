using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using log4net;
using SharedClasses;
using SharedClasses.Message;
using SharedClasses.Serialiser;
using SharedClasses.Serialiser.MessageSerialiser;

namespace ChatClient.Services
{
    /// <summary>
    /// Creates a connection to the Server and initialises the <see cref="repositoryManager"/> repositories.
    /// </summary>
    internal sealed class ServerLoginHandler
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (ServerLoginHandler));

        private readonly RepositoryManager repositoryManager;
        private readonly SerialiserFactory serialiserFactory = new SerialiserFactory();

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
            var loginResponse = (LoginResponse) GetConnectionIMessage(loginConnection);
            return loginResponse;
        }

        public void BootstrapRepositories(int userId)
        {
            SendConnectionMessage(new UserSnapshotRequest(userId), loginConnection);

            var userSnapshot = (UserSnapshot) GetConnectionIMessage(loginConnection);

            SendConnectionMessage(new ConversationSnapshotRequest(userId), loginConnection);

            var conversationSnapshot = (ConversationSnapshot) GetConnectionIMessage(loginConnection);

            SendConnectionMessage(new ParticipationSnapshotRequest(userId), loginConnection);

            var participationSnapshot = (ParticipationSnapshot) GetConnectionIMessage(loginConnection);

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
            messageSerialiser.Serialise(tcpClient.GetStream(), message);
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