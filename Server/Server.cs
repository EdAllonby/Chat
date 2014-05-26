using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using log4net;
using SharedClasses;
using SharedClasses.Domain;
using SharedClasses.Message;
using SharedClasses.Serialiser;

namespace Server
{
    /// <summary>
    /// Handles the logic for incoming <see cref="IMessage"/> passed from ConnectionHandler.
    /// </summary>
    internal sealed class Server
    {
        private const int PortNumber = 5004;
        private static readonly ILog Log = LogManager.GetLogger(typeof (Server));

        private readonly IDictionary<int, ConnectionHandler> clientConnectionHandlersIndexedByUserId = new Dictionary<int, ConnectionHandler>();

        private readonly ConversationRepository conversationRepository = new ConversationRepository();
        private readonly EntityGeneratorFactory entityIDGenerator = new EntityGeneratorFactory();

        private readonly ParticipationRepository participationRepository = new ParticipationRepository();

        private readonly SerialiserFactory serialiserFactory = new SerialiserFactory();
        private readonly UserRepository userRepository = new UserRepository();
        
        public Server()
        {
            Log.Info("Server instance started");
            ListenForNewClients();
        }

        private void ListenForNewClients()
        {
            var clientListener = new TcpListener(IPAddress.Any, PortNumber);
            clientListener.Start();
            Log.Info("Server started listening for clients to connect");

            while (true)
            {
                TcpClient client = clientListener.AcceptTcpClient();
                Log.Info("New client connected");
                client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
                var clientLoginWorkerThread = new Thread(() => InitialiseNewClient(client))
                {
                    Name = "Client Login Worker Thread"
                };

                clientLoginWorkerThread.Start();
            }
        }

        private void InitialiseNewClient(TcpClient tcpClient)
        {
            LoginRequest loginRequest = GetLoginRequest(tcpClient);

            User user = userRepository.FindUserByUsername(loginRequest.User.Username);

            if (user == null || user.ConnectionStatus != ConnectionStatus.Connected)
            {
                if (user == null)
                {
                    user = CreateUserEntity(loginRequest);
                    NotifyClientsOfUser(user, NotificationType.Create, ConnectionStatus.Connected);
                }
                else
                {
                    NotifyClientsOfUser(user, NotificationType.Update, ConnectionStatus.Connected);
                }
                var loginResponse = new LoginResponse(user, LoginResult.Success);

                SendConnectionMessage(loginResponse, tcpClient);

                SendClientSnapshots(tcpClient, user.UserId);

                var connectionHandler = new ConnectionHandler(user.UserId, tcpClient);

                connectionHandler.OnNewMessage += NewMessageReceived;

                clientConnectionHandlersIndexedByUserId[user.UserId] = connectionHandler;
            }
            else
            {
                Log.InfoFormat("User with user Id {0} already connected, denying user login.", user.UserId);
                var loginResponse = new LoginResponse(null, LoginResult.AlreadyConnected);
                SendConnectionMessage(loginResponse, tcpClient);
            }
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
            IMessage message = GetIMessage(tcpClient);
            if (message.Identifier != messageNumber)
            {
                throw new IOException();
            }
        }

        private IMessage GetIMessage(TcpClient tcpClient)
        {
            var messageIdentifierSerialiser = new MessageIdentifierSerialiser();

            MessageNumber messageIdentifier = messageIdentifierSerialiser.DeserialiseMessageIdentifier(tcpClient.GetStream());

            ISerialiser serialiser = serialiserFactory.GetSerialiser(messageIdentifier);

            return serialiser.Deserialise(tcpClient.GetStream());
        }

        private static LoginRequest GetLoginRequest(TcpClient tcpClient)
        {
            var messageIdentifierSerialiser = new MessageIdentifierSerialiser();

            MessageNumber messageIdentifier = messageIdentifierSerialiser.DeserialiseMessageIdentifier(tcpClient.GetStream());

            var serialiserFactory = new SerialiserFactory();

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

        private int CreateConversationEntity(ConversationRequest conversationRequest)
        {
            int conversationId = entityIDGenerator.GetEntityID<Conversation>();

            var newConversation = new Conversation(conversationId);

            foreach (int participantId in conversationRequest.ParticipantIds)
            {
                participationRepository.AddParticipation(new Participation(participantId, conversationId));
            }

            conversationRepository.AddConversation(newConversation);

            return conversationId;
        }

        private Contribution CreateContributionEntity(ContributionRequest contributionRequest)
        {
            var newContribution = new Contribution(entityIDGenerator.GetEntityID<Contribution>(),
                contributionRequest.Contribution);

            Conversation conversation = conversationRepository.FindConversationById(newContribution.ConversationId);

            conversation.AddContribution(newContribution);

            return newContribution;
        }

        private void NotifyClientsOfUser(User user, NotificationType updateType, ConnectionStatus status)
        {
            user.ConnectionStatus = status;

            var userNotification = new UserNotification(user, updateType);

            foreach (ConnectionHandler handler in clientConnectionHandlersIndexedByUserId.Values)
            {
                handler.SendMessage(userNotification);
            }
        }

        private void NewMessageReceived(object sender, MessageEventArgs e)
        {
            IMessage message = e.Message;

            switch (message.Identifier)
            {
                case MessageNumber.ContributionRequest:
                    Contribution contribution = CreateContributionEntity((ContributionRequest) message);
                    SendContributionNotificationToParticipants(contribution);
                    break;

                case MessageNumber.ClientDisconnection:
                    RemoveClientHandler(e.ClientUserId);
                    NotifyClientsOfUser(userRepository.FindUserByID(e.ClientUserId), NotificationType.Update,
                        ConnectionStatus.Disconnected);
                    break;

                case MessageNumber.ConversationRequest:
                    if (CheckConversationIsValid((ConversationRequest) message))
                    {
                        var conversationRequest = (ConversationRequest) message;
                        int conversationId = CreateConversationEntity(conversationRequest);
                        SendConversationNotificationToClients(conversationRequest.ParticipantIds, conversationId);
                    }
                    break;

                default:
                    Log.Warn("Server is not supposed to handle message with identifier: " + message.Identifier);
                    break;
            }
        }

        private void SendConnectionMessage(IMessage message, TcpClient tcpClient)
        {
            ISerialiser messageSerialiser = serialiserFactory.GetSerialiser(message.Identifier);
            messageSerialiser.Serialise(message, tcpClient.GetStream());
        }

        private bool CheckConversationIsValid(ConversationRequest conversationRequest)
        {
            // Check for no repeating users
            if (conversationRequest.ParticipantIds.Count != conversationRequest.ParticipantIds.Distinct().Count())
            {
                Log.Warn("Cannot make a conversation between two users of same id");
                return false;
            }

            return !participationRepository.DoesConversationWithUsersExist(conversationRequest.ParticipantIds);
        }

        private void SendConversationNotificationToClients(List<int> participantIds, int conversationId)
        {
            var conversationNotification = new ConversationNotification(participantIds, conversationId);

            // Send message to each user in conversation
            foreach (ConnectionHandler participantConnectionHandler in participantIds
                .Select(participant => clientConnectionHandlersIndexedByUserId[participant]))
            {
                participantConnectionHandler.SendMessage(conversationNotification);
            }
        }

        private void SendContributionNotificationToParticipants(Contribution contribution)
        {
            var contributionNotification = new ContributionNotification(contribution);
            Conversation conversation = conversationRepository.FindConversationById(contribution.ConversationId);

            foreach (User user in participationRepository.GetParticipationsByConversationId(conversation.ConversationId)
                .Select(participant => userRepository.FindUserByID(participant.UserId))
                .Where(user => user.ConnectionStatus == ConnectionStatus.Connected))
            {
                clientConnectionHandlersIndexedByUserId[user.UserId].SendMessage(contributionNotification);
            }
        }

        private void RemoveClientHandler(int userId)
        {
            clientConnectionHandlersIndexedByUserId.Remove(userId);
            Log.Info("User with id " + userId + " logged out. Removing from Server's ConnectionHandler list");
        }
    }
}