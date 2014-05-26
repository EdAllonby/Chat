using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using log4net;
using SharedClasses;
using SharedClasses.Domain;
using SharedClasses.Message;
using SharedClasses.Serialiser;

namespace Server
{
    internal sealed class ClientLoginHandler
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (ClientLoginHandler));

        private readonly ClientHandler clientHandler;
        private readonly EntityGeneratorFactory entityIDGenerator;
        private readonly UserRepository userRepository;
        private readonly ConversationRepository conversationRepository;
        private readonly ParticipationRepository participationRepository;

        private readonly SerialiserFactory serialiserFactory = new SerialiserFactory();

        public ClientLoginHandler(ClientHandler clientHandler,
            EntityGeneratorFactory entityIDGenerator,
            UserRepository userRepository,
            ConversationRepository conversationRepository,
            ParticipationRepository participationRepository)
        {
            this.clientHandler = clientHandler;
            this.entityIDGenerator = entityIDGenerator;
            this.userRepository = userRepository;
            this.conversationRepository = conversationRepository;
            this.participationRepository = participationRepository;
        }

        public LoginResponse InitialiseNewClient(TcpClient tcpClient)
        {
            LoginRequest loginRequest = GetLoginRequest(tcpClient);
            User user = userRepository.FindUserByUsername(loginRequest.User.Username);

            LoginResponse loginResponse;

            if (user == null || user.ConnectionStatus != ConnectionStatus.Connected)
            {
                if (user == null)
                {
                    user = CreateUserEntity(loginRequest);

                    user.ConnectionStatus = ConnectionStatus.Connected;
                    var userNotification = new UserNotification(user, NotificationType.Create);
                    clientHandler.NotifyAllClients(userNotification);
                }
                else
                {
                    user.ConnectionStatus = ConnectionStatus.Connected;
                    var userNotification = new UserNotification(user, NotificationType.Create);
                    clientHandler.NotifyAllClients(userNotification);
                }
                loginResponse = new LoginResponse(user, LoginResult.Success);

                SendConnectionMessage(loginResponse, tcpClient);

                SendClientSnapshots(tcpClient, user.UserId);
            }
            else
            {
                Log.InfoFormat("User with user Id {0} already connected, denying user login.", user.UserId);
                loginResponse = new LoginResponse(null, LoginResult.AlreadyConnected);
                SendConnectionMessage(loginResponse, tcpClient);
            }
         
            return loginResponse;
        }

        private LoginRequest GetLoginRequest(TcpClient tcpClient)
        {
            var messageIdentifierSerialiser = new MessageIdentifierSerialiser();

            MessageNumber messageIdentifier = messageIdentifierSerialiser.DeserialiseMessageIdentifier(tcpClient.GetStream());

            ISerialiser serialiser = serialiserFactory.GetSerialiser(messageIdentifier);

            var loginRequest = (LoginRequest) serialiser.Deserialise(tcpClient.GetStream());

            return loginRequest;
        }

        private User CreateUserEntity(LoginRequest clientLogin)
        {
            var newUser = new User(clientLogin.User.Username, entityIDGenerator.GetEntityID<User>(), ConnectionStatus.Connected);

            userRepository.AddUser(newUser);

            return newUser;
        }

        private void SendConnectionMessage(IMessage message, TcpClient tcpClient)
        {
            ISerialiser messageSerialiser = serialiserFactory.GetSerialiser(message.Identifier);
            messageSerialiser.Serialise(message, tcpClient.GetStream());
        }

        private void SendClientSnapshots(TcpClient tcpClient, int userId)
        {
            CheckStreamContainsMessage(tcpClient, MessageNumber.UserSnapshotRequest);

            SendUserSnapshot(tcpClient);

            CheckStreamContainsMessage(tcpClient, MessageNumber.ConversationSnapshotRequest);

            IEnumerable<int> conversations = SendConversationSnapshot(tcpClient, userId);

            CheckStreamContainsMessage(tcpClient, MessageNumber.ParticipationSnapshotRequest);

            SendParticipationSnapshot(tcpClient, conversations);
        }

        private void SendParticipationSnapshot(TcpClient tcpClient, IEnumerable<int> conversations)
        {
            var userParticipations = new List<Participation>();

            foreach (int conversationId in conversations)
            {
                userParticipations.AddRange(participationRepository.GetParticipationsByConversationId(conversationId));
            }

            var participationSnapshot = new ParticipationSnapshot(userParticipations);

            SendConnectionMessage(participationSnapshot, tcpClient);
        }

        private IEnumerable<int> SendConversationSnapshot(TcpClient tcpClient, int userId)
        {
            IEnumerable<int> conversationIds = participationRepository.GetAllConversationIdsByUserId(userId);

            IList<int> conversationEnumerable = conversationIds as IList<int> ?? conversationIds.ToList();

            List<Conversation> conversations =
                conversationEnumerable.Select(conversationId => conversationRepository.FindConversationById(conversationId)).ToList();

            var conversationSnapshot = new ConversationSnapshot(conversations);

            SendConnectionMessage(conversationSnapshot, tcpClient);
            return conversationEnumerable;
        }

        private void SendUserSnapshot(TcpClient tcpClient)
        {
            IEnumerable<User> currentUsers = userRepository.GetAllUsers();
            var userSnapshot = new UserSnapshot(currentUsers);

            SendConnectionMessage(userSnapshot, tcpClient);
        }

        private void CheckStreamContainsMessage(TcpClient tcpClient, MessageNumber messageNumber)
        {
            IMessage message = GetConnectionIMessage(tcpClient);
            if (message.Identifier != messageNumber)
            {
                throw new IOException();
            }
        }

        private IMessage GetConnectionIMessage(TcpClient tcpClient)
        {
            var messageIdentifierSerialiser = new MessageIdentifierSerialiser();

            MessageNumber messageIdentifier = messageIdentifierSerialiser.DeserialiseMessageIdentifier(tcpClient.GetStream());

            ISerialiser serialiser = serialiserFactory.GetSerialiser(messageIdentifier);

            return serialiser.Deserialise(tcpClient.GetStream());
        }
    }
}