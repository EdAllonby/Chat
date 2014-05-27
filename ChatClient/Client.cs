using System.Collections.Generic;
using log4net;
using SharedClasses;
using SharedClasses.Domain;
using SharedClasses.Message;

namespace ChatClient
{
    /// <summary>
    /// Handles the logic for <see cref="IMessage" />
    /// Delegates Server specific communications to the <see cref="connectionHandler" />
    /// </summary>
    public sealed class Client
    {
        public delegate void NewContributionNotificationHandler(Conversation contributions);

        public delegate void NewConversationHandler(Conversation conversation);

        public delegate void UserListHandler(IEnumerable<User> users);

        private static readonly ILog Log = LogManager.GetLogger(typeof (Client));

        private readonly ConversationRepository conversationRepository = new ConversationRepository();

        private readonly ParticipationRepository participationRepository = new ParticipationRepository();

        private ConnectionHandler connectionHandler;
        private readonly UserRepository userRepository = new UserRepository();

        public int ClientUserId { get; private set; }

        public event UserListHandler OnNewUser = delegate { };
        public event NewConversationHandler OnNewConversationNotification = delegate { };
        public event NewContributionNotificationHandler OnNewContributionNotification = delegate { };

        private void NotifyClientOfUserChange()
        {
            OnNewUser(userRepository.GetAllUsers());
            Log.Info("User changed event fired");
        }

        /// <summary>
        /// Connects the Client to the server using the parameters as connection details
        /// and gets the state of <see cref="Client"/> up to date with the user status'. 
        /// </summary>
        /// <param name="loginDetails">The details used to log in to the Chat Program.</param>
        public LoginResult ConnectToServer(LoginDetails loginDetails)
        {
            var loginHandler = new ServerLoginHandler();

            LoginResponse response = loginHandler.ConnectToServer(loginDetails);
            
            if (response.LoginResult == LoginResult.Success)
            {
                InitialisedData initialisedData = loginHandler.GetSnapshots(response.User.UserId);

                connectionHandler = loginHandler.CreateServerConnectionHandler(response.User.UserId);

                Log.DebugFormat("Connection process to the server has finished");

                ClientUserId = initialisedData.UserId;

                userRepository.AddUsers(initialisedData.UserSnapshot.Users);

                conversationRepository.AddConversations(initialisedData.ConversationSnapshot.Conversations);

                participationRepository.AddParticipations(initialisedData.ParticipationSnapshot.Participations);

                connectionHandler.OnNewMessage += NewMessageReceived;
            }
            else
            {
                Log.WarnFormat("User {0} already connected.", loginDetails.Username);
            }

            return response.LoginResult;
        }

        /// <summary>
        /// Sends a <see cref="ConversationRequest"/> message to the server.
        /// </summary>
        /// <param name="participantIds">The participants that are included in the conversation.</param>
        public void SendConversationRequest(List<int> participantIds)
        {
            var conversationRequest = new ConversationRequest(participantIds);
            connectionHandler.SendMessage(conversationRequest);
        }

        /// <summary>
        /// Sends a <see cref="ContributionRequest"/> message to the server.
        /// </summary>
        /// <param name="conversationID">The ID of the conversation the Client wants to send the message to.</param>
        /// <param name="message">The content of the message.</param>
        public void SendContributionRequest(int conversationID, string message)
        {
            var clientContribution = new ContributionRequest(conversationID, ClientUserId, message);
            connectionHandler.SendMessage(clientContribution);
        }

        /// <summary>
        /// Returns all <see cref="User"/>s that the client knows of.
        /// </summary>
        /// <returns>A collection of all <see cref="User"/>s.</returns>
        public IEnumerable<User> GetAllUsers()
        {
            return userRepository.GetAllUsers();
        }

        /// <summary>
        /// Returns all <see cref="Participation"/>s that the client knows of.
        /// </summary>
        /// <returns>A collection of all <see cref="Participation"/>s.</returns>
        public IEnumerable<Participation> GetAllParticipations()
        {
            return participationRepository.GetAllParticipations();
        }

        /// <summary>
        /// Returns the <see cref="User"/> entity object that is specific to this client.
        /// </summary>
        /// <param name="userId">The Id that matches the <see cref="User"/>.</param>
        /// <returns>The <see cref="User"/> that matches the <see cref="User"/> Id.</returns>
        public User GetUser(int userId)
        {
            return userRepository.FindUserByID(userId);
        }

        /// <summary>
        /// Gets the <see cref="Conversation"/> object that matches the <see cref="Conversation"/> Id.
        /// </summary>
        /// <param name="conversationId">The Id that matches the <see cref="Conversation"/>.</param>
        /// <returns>The <see cref="Conversation"/> that matches the <see cref="Conversation"/> Id.</returns>
        public Conversation GetConversation(int conversationId)
        {
            return conversationRepository.FindConversationById(conversationId);
        }

        /// <summary>
        /// Checks whether a <see cref="Conversation"/> exists for a particular set of <see cref="User"/>s.
        /// </summary>
        /// <param name="participantIds">The set of <see cref="User"/> Ids to check for a conversation.</param>
        /// <returns>Whether a conversation exists for the set of <see cref="User"/>s.</returns>
        public bool DoesConversationExist(IEnumerable<int> participantIds)
        {
            return participationRepository.DoesConversationWithUsersExist(participantIds);
        }

        /// <summary>
        /// Gets the <see cref="Conversation"/> Id for the particular set of <see cref="User"/>s.
        /// </summary>
        /// <param name="participantIds">The set of <see cref="User"/> Ids that belong to the conversation.</param>
        /// <returns>The <see cref="Conversation"/> Id.</returns>
        public int GetConversationId(IEnumerable<int> participantIds)
        {
            return participationRepository.GetConversationIdByParticipantsId(participantIds);
        }

        private void NewMessageReceived(object sender, MessageEventArgs e)
        {
            switch (e.Message.Identifier)
            {
                case MessageNumber.ContributionNotification:
                    var contributionNotification = (ContributionNotification) e.Message;
                    AddContributionToConversation(contributionNotification);
                    break;

                case MessageNumber.UserNotification:
                    UpdateUserRepository((UserNotification) e.Message);
                    NotifyClientOfUserChange();
                    break;

                case MessageNumber.ConversationNotification:
                    var conversationNotification = (ConversationNotification) e.Message;
                    AddParticipants(conversationNotification);
                    AddConversationToRepository((ConversationNotification) e.Message);
                    break;

                default:
                    Log.Warn("Client is not supposed to handle message with identifier: " + e.Message.Identifier);
                    break;
            }
        }

        private void AddParticipants(ConversationNotification conversationNotification)
        {
            foreach (int participantId in conversationNotification.ParticipantIds)
            {
                participationRepository.AddParticipation(new Participation(participantId,
                    conversationNotification.ConversationId));
            }
        }

        private void AddContributionToConversation(ContributionNotification contributionNotification)
        {
            Conversation conversation =
                conversationRepository.FindConversationById(contributionNotification.Contribution.ConversationId);

            conversation.AddContribution(contributionNotification);
            OnNewContributionNotification(conversation);
        }

        private void AddConversationToRepository(ConversationNotification conversationNotification)
        {
            var conversation = new Conversation(conversationNotification.ConversationId);
            conversationRepository.AddConversation(conversation);
            OnNewConversationNotification(conversation);
        }

        private void UpdateUserRepository(UserNotification userNotification)
        {
            userRepository.AddUser(userNotification.User);
        }
    }
}