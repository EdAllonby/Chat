using System.Collections.Generic;
using System.Drawing;
using ChatClient.Services.MessageHandler;
using log4net;
using SharedClasses;
using SharedClasses.Domain;
using SharedClasses.Message;

namespace ChatClient.Services
{
    /// <summary>
    /// Handles the logic for <see cref="IMessage" />
    /// Delegates Server specific communications to the <see cref="connectionHandler" />
    /// </summary>
    public sealed class ClientService : IClientService
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (ClientService));

        private readonly ClientContextRegistry clientContextRegistry;
        private readonly RepositoryManager repositoryManager = new RepositoryManager();

        private ConnectionHandler connectionHandler;

        /// <summary>
        /// Initialises a new <see cref="ClientService"/>.
        /// </summary>
        public ClientService()
        {
            clientContextRegistry = new ClientContextRegistry(repositoryManager);
        }

        /// <summary>
        /// This Client's unique User Id.
        /// </summary>
        public int ClientUserId { get; private set; }

        /// <summary>
        /// Holds the repositories for the Client.
        /// </summary>
        public RepositoryManager RepositoryManager
        {
            get { return repositoryManager; }
        }

        /// <summary>
        /// Connects the Client to the server using the parameters as connection details
        /// and gets the state of <see cref="ClientService"/> up to date with the user status'. 
        /// </summary>
        /// <param name="loginDetails">The details used to log in to the Chat Program.</param>
        public LoginResult LogOn(LoginDetails loginDetails)
        {
            var serverLoginHandler = new ServerLoginHandler(repositoryManager);

            LoginResponse response = serverLoginHandler.ConnectToServer(loginDetails, out connectionHandler);

            if (response.LoginResult == LoginResult.Success)
            {
                ClientUserId = response.User.UserId;

                connectionHandler.MessageReceived += OnNewMessageReceived;
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
        /// <param name="userIds">The participants that are included in the conversation.</param>
        public void CreateConversation(List<int> userIds)
        {
            connectionHandler.SendMessage(new ConversationRequest(userIds));
        }

        /// <summary>
        /// Sends a <see cref="ParticipationRequest"/> message to the server to add a user to an existing conversation.
        /// </summary>
        /// <param name="userId">The participant that will be added to the conversation.</param>
        /// <param name="conversationId">The targetted conversation the participant will be added to.</param>
        public void AddUserToConversation(int userId, int conversationId)
        {
            connectionHandler.SendMessage(new ParticipationRequest(new Participation(userId, conversationId)));
        }

        /// <summary>
        /// Sends a <see cref="ContributionRequest"/> message to the server.
        /// </summary>
        /// <param name="conversationId">The ID of the conversation the Client wants to send the message to.</param>
        /// <param name="message">The content of the message.</param>
        public void SendContribution(int conversationId, string message)
        {
            connectionHandler.SendMessage(new ContributionRequest(conversationId, ClientUserId, message));
        }

        /// <summary>
        /// Sends an <see cref="AvatarRequest"/> message to the server to change a user's avatar.
        /// </summary>
        /// <param name="avatar">The new image the user requests to have.</param>
        public void SendAvatarRequest(Image avatar)
        {
            connectionHandler.SendMessage(new AvatarRequest(ClientUserId, avatar));
        }

        private void OnNewMessageReceived(object sender, MessageEventArgs e)
        {
            IMessage message = e.Message;

            try
            {
                IMessageHandler handler =
                    MessageHandlerRegistry.MessageHandlersIndexedByMessageIdentifier[message.MessageIdentifier];

                IMessageContext context =
                    clientContextRegistry.MessageHandlersIndexedByMessageIdentifier[message.MessageIdentifier];

                handler.HandleMessage(message, context);
            }
            catch (KeyNotFoundException keyNotFoundException)
            {
                Log.Error("ClientService is not supposed to handle message with identifier: " + e.Message.MessageIdentifier,
                    keyNotFoundException);
            }
        }
    }
}