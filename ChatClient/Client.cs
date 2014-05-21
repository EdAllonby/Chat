using System.Collections.Generic;
using System.Linq;
using System.Net;
using log4net;
using SharedClasses;
using SharedClasses.Domain;
using SharedClasses.Message;

namespace ChatClient
{
    /// <summary>
    /// Handles the logic for <see cref="IMessage" />
    /// Delegates Server specific communications to the <see cref="serverHandler" />
    /// </summary>
    public sealed class Client
    {
        public delegate void LoginCompleteHandler();

        public delegate void NewContributionNotificationHandler(Conversation contributions);

        public delegate void NewConversationHandler(Conversation conversation);

        public delegate void UserListHandler(IEnumerable<User> users);

        private static readonly ILog Log = LogManager.GetLogger(typeof (Client));


        private readonly UserRepository userRepository = new UserRepository();

        private readonly ConversationRepository conversationRepository = new ConversationRepository();

        private readonly ParticipationRepository participationRepository = new ParticipationRepository();

        private readonly ServerHandler serverHandler = new ServerHandler();

        public UserRepository UserRepository
        {
            get { return userRepository; }
        }

        public ConversationRepository ConversationRepository
        {
            get { return conversationRepository; }
        }

        public ParticipationRepository ParticipationRepository
        {
            get { return participationRepository; }
        }

        public int ClientUserId { get; private set; }

        public event UserListHandler OnNewUser = delegate { };
        public event NewConversationHandler OnNewConversationNotification = delegate { };
        public event NewContributionNotificationHandler OnNewContributionNotification = delegate { };
        public event LoginCompleteHandler OnLoginComplete = delegate { };

        private void NotifyClientOfUserChange()
        {
            OnNewUser(userRepository.GetAllEntities());
            Log.Info("User changed event fired");
        }

        /// <summary>
        /// Connects the Client to the server using the parameters as connection details
        /// and gets the state of <see cref="Client"/> up to date with the user status'. 
        /// </summary>
        /// <param name="username">The name the user wants to have.</param>
        /// <param name="targetAddress">The address of the Server.</param>
        /// <param name="targetPort">The port the Server is running on.</param>
        public void ConnectToServer(string username, IPAddress targetAddress, int targetPort)
        {
            Snapshots snapshots = serverHandler.ConnectToServer(username, targetAddress, targetPort);

            UserRepository.AddUsers(snapshots.UserSnapshot.Users);

            ConversationRepository.AddConversations(snapshots.ConversationSnapshot.Conversations);

            ParticipationRepository.AddParticipations(snapshots.ParticipationSnapshot.Participations);

            SetClientUserID(username);

            serverHandler.OnNewMessage += NewMessageReceived;

            OnLoginComplete();
        }

        private void SetClientUserID(string username)
        {
            IEnumerable<User> users = UserRepository.GetAllEntities();

            foreach (User user in users.Where(user => user.Username == username))
            {
                ClientUserId = user.UserId;
            }
        }

        /// <summary>
        /// Sends a <see cref="ConversationRequest"/> message to the server.
        /// </summary>
        /// <param name="participantIds">The participants that are included in the conversation.</param>
        public void SendConversationRequest(List<int> participantIds)
        {
            var conversationRequest = new ConversationRequest(participantIds);
            serverHandler.SendMessage(conversationRequest);
        }

        /// <summary>
        /// Sends a <see cref="ContributionRequest"/> message to the server.
        /// </summary>
        /// <param name="conversationID">The ID of the conversation the Client wants to send the message to.</param>
        /// <param name="message">The content of the message.</param>
        public void SendContributionRequest(int conversationID, string message)
        {
            var clientContribution = new ContributionRequest(conversationID, ClientUserId, message);
            serverHandler.SendMessage(clientContribution);
        }

        private void NewMessageReceived(object sender, MessageEventArgs e)
        {
            IMessage message = e.Message;

            switch (message.Identifier)
            {
                case MessageNumber.ContributionNotification:
                    var contributionNotification = (ContributionNotification) message;
                    AddContributionToConversation(contributionNotification);
                    break;

                case MessageNumber.UserNotification:
                    UpdateUserRepository((UserNotification) message);
                    NotifyClientOfUserChange();
                    break;

                case MessageNumber.ConversationNotification:
                    var conversationNotification = (ConversationNotification) message;
                    AddParticipants(conversationNotification);
                    AddConversationToRepository((ConversationNotification) message);
                    break;

                default:
                    Log.Warn("Client is not supposed to handle message with identifier: " + message.Identifier);
                    break;
            }
        }

        private void AddParticipants(ConversationNotification conversationNotification)
        {
            foreach (int participantId in conversationNotification.ParticipantIds)
            {
                ParticipationRepository.AddParticipation(new Participation(participantId, conversationNotification.ConversationId));
            }
        }

        private void AddContributionToConversation(ContributionNotification contributionNotification)
        {
            Conversation conversation = conversationRepository
                .FindEntityByID(contributionNotification.Contribution.ConversationId);

            conversation.AddContribution(contributionNotification);
            OnNewContributionNotification(conversation);
        }

        private void AddConversationToRepository(ConversationNotification conversationNotification)
        {
            var conversation = new Conversation(conversationNotification.ConversationId);
            conversationRepository.AddEntity(conversation);
            OnNewConversationNotification(conversation);
        }

        private void UpdateUserRepository(UserNotification userNotification)
        {
            switch (userNotification.Notification)
            {
                case NotificationType.Create:
                    userRepository.AddEntity(userNotification.User);
                    break;
                case NotificationType.Update:
                    userRepository.UpdateEntity(userNotification.User);
                    break;
            }
        }
    }
}