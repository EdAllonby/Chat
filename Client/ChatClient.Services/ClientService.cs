using System.Collections.Generic;
using log4net;
using SharedClasses;
using SharedClasses.Domain;
using SharedClasses.Message;

namespace ChatClient.Services
{
    public delegate void NewContributionNotificationHandler(Conversation contributions);

    public delegate void NewConversationHandler(Conversation conversation);

    public delegate void UserListHandler(IEnumerable<User> users);

    /// <summary>
    /// Handles the logic for <see cref="IMessage" />
    /// Delegates Server specific communications to the <see cref="connectionHandler" />
    /// </summary>
    public sealed class ClientService : IClientService
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (ClientService));

        private readonly RepositoryManager repositoryManager = new RepositoryManager();
        private ConnectionHandler connectionHandler;

        public int ClientUserId { get; private set; }

        /// <summary>
        /// Holds the repositories for the Client.
        /// </summary>
        public RepositoryManager RepositoryManager
        {
            get { return repositoryManager; }
        }

        public event UserListHandler NewUser = delegate { };
        public event NewConversationHandler NewConversationNotification = delegate { };
        public event NewContributionNotificationHandler NewContributionNotification = delegate { };

        /// <summary>
        /// Connects the Client to the server using the parameters as connection details
        /// and gets the state of <see cref="ClientService"/> up to date with the user status'. 
        /// </summary>
        /// <param name="loginDetails">The details used to log in to the Chat Program.</param>
        public LoginResult LogOn(LoginDetails loginDetails)
        {
            var loginHandler = new ServerLoginHandler(repositoryManager);

            LoginResponse response = loginHandler.ConnectToServer(loginDetails);

            if (response.LoginResult == LoginResult.Success)
            {
                loginHandler.GetSnapshots(response.User.UserId);

                connectionHandler = loginHandler.CreateServerConnectionHandler(response.User.UserId);

                Log.DebugFormat("Connection process to the server has finished");

                ClientUserId = response.User.UserId;

                connectionHandler.MessageReceived += NewMessageReceived;
            }
            else
            {
                Log.WarnFormat("User {0} already connected.", loginDetails.Username);
            }

            return response.LoginResult;
        }

        /// <summary>
        /// Sends a <see cref="NewConversationRequest"/> message to the server.
        /// </summary>
        /// <param name="participantIds">The participants that are included in the conversation.</param>
        public void SendConversationRequest(List<int> participantIds)
        {
            var conversationRequest = new NewConversationRequest(participantIds);
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

        private void NewMessageReceived(object sender, MessageEventArgs e)
        {
            switch (e.Message.MessageIdentifier)
            {
                case MessageIdentifier.ContributionNotification:
                    var contributionNotification = (ContributionNotification) e.Message;
                    AddContributionToConversation(contributionNotification);
                    break;

                case MessageIdentifier.UserNotification:
                    UpdateUserRepository((UserNotification) e.Message);
                    NotifyClientOfUserChange();
                    break;

                case MessageIdentifier.ConversationNotification:
                    var conversationNotification = (ConversationNotification) e.Message;
                    AddConversationToRepository(conversationNotification);
                    break;

                case MessageIdentifier.ParticipationsNotification:
                    var participantsNotification = (ParticipationsNotification) e.Message;
                    AddParticipants(participantsNotification);
                    break;

                default:
                    Log.Warn("ClientService is not supposed to handle message with identifier: " + e.Message.MessageIdentifier);
                    break;
            }
        }

        private void AddParticipants(ParticipationsNotification participationsNotification)
        {
            foreach (int participantId in participationsNotification.ParticipantIds)
            {
                repositoryManager.ParticipationRepository.AddParticipation(new Participation(participantId,
                    participationsNotification.ConversationId));
            }
        }

        private void AddContributionToConversation(ContributionNotification contributionNotification)
        {
            Conversation conversation = repositoryManager.ConversationRepository.FindConversationById(contributionNotification.Contribution.ConversationId);

            conversation.AddContribution(contributionNotification);
            NewContributionNotification(conversation);
        }

        private void AddConversationToRepository(ConversationNotification conversationNotification)
        {
            var conversation = new Conversation(conversationNotification.Conversation.ConversationId);
            repositoryManager.ConversationRepository.AddConversation(conversation);
            NewConversationNotification(conversation);
        }

        private void UpdateUserRepository(UserNotification userNotification)
        {
            repositoryManager.UserRepository.UpdateUser(userNotification.User);
        }

        private void NotifyClientOfUserChange()
        {
            NewUser(repositoryManager.UserRepository.GetAllUsers());
            Log.Info("User changed event fired");
        }
    }
}