using System;
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
    /// Handles the logic for <see cref="IMessage" />.
    /// Delegates Server specific communications to the <see cref="connectionHandler" />.
    /// </summary>
    public sealed class ClientService : IClientService
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(ClientService));
        private readonly IServiceRegistry serviceRegistry;
        private readonly MessageThroughputLimiter<UserTypingRequest> userTypingThroughputLimiter;
        private ConnectionHandler connectionHandler;
        private MessageHandlerRegistry messageHandlerRegistry;

        /// <summary>
        /// Passes the service the reference to the <see cref="IServiceRegistry" />.
        /// </summary>
        /// <param name="serviceRegistry">Contains a housing for client services.</param>
        public ClientService(IServiceRegistry serviceRegistry)
        {
            messageHandlerRegistry = new MessageHandlerRegistry(serviceRegistry);
            this.serviceRegistry = serviceRegistry;
            const int minimumMillisecondsAllowedBetweenUserTypingMessages = 1000;
            userTypingThroughputLimiter = new MessageThroughputLimiter<UserTypingRequest>(minimumMillisecondsAllowedBetweenUserTypingMessages);
            userTypingThroughputLimiter.DeferredSendLastMessage += userTypingThroughputLimiter_DeferredSendLastMessage;
        }

        /// <summary>
        /// This client's EntityRepository Manager.
        /// </summary>
        public RepositoryManager RepositoryManager => serviceRegistry.GetService<RepositoryManager>();

        /// <summary>
        /// Gets fired when bootstrapping the repository is complete.
        /// </summary>
        public event EventHandler BootstrapCompleted;

        /// <summary>
        /// This Client's unique User Id.
        /// </summary>
        public int ClientUserId { get; private set; }

        /// <summary>
        /// Connects the Client to the server using the parameters as connection details
        /// and gets the state of <see cref="ClientService" /> up to date with the user status'.
        /// </summary>
        /// <param name="loginDetails">The details used to log in to the Chat Program.</param>
        public LoginResult LogOn(LoginDetails loginDetails)
        {
            var serverLoginHandler = new ServerLoginHandler(messageHandlerRegistry);
            serverLoginHandler.BootstrapCompleted += OnBootstrapCompleted;

            LoginResponse response = serverLoginHandler.ConnectToServer(loginDetails, out connectionHandler);

            switch (response.LoginResult)
            {
                case LoginResult.Success:
                    ClientUserId = response.User.Id;
                    connectionHandler.MessageReceived += OnNewMessageReceived;
                    break;
                case LoginResult.AlreadyConnected:
                    Log.WarnFormat($"User {loginDetails.Username} already connected.");
                    break;
                case LoginResult.ServerNotFound:
                    Log.WarnFormat("Cannot find server.");
                    break;
            }

            return response.LoginResult;
        }

        /// <summary>
        /// Sends a <see cref="ConversationRequest" /> message to the server.
        /// </summary>
        /// <param name="userIds">The participants that are included in the conversation.</param>
        public void CreateConversation(List<int> userIds)
        {
            connectionHandler.SendMessage(new ConversationRequest(userIds));
        }

        /// <summary>
        /// Sends a <see cref="ParticipationRequest" /> message to the server to add a user to an existing conversation.
        /// </summary>
        /// <param name="userId">The participant that will be added to the conversation.</param>
        /// <param name="conversationId">The targetted conversation the participant will be added to.</param>
        public void AddUserToConversation(int userId, int conversationId)
        {
            connectionHandler.SendMessage(new ParticipationRequest(new Participation(userId, conversationId)));
        }

        /// <summary>
        /// Sends a <see cref="ContributionRequest" /> message to the server.
        /// </summary>
        /// <param name="conversationId">The ID of the conversation the Client wants to send the message to.</param>
        /// <param name="message">The content of the message.</param>
        public void SendContribution(int conversationId, string message)
        {
            connectionHandler.SendMessage(new ContributionRequest(new TextContribution(ClientUserId, message, conversationId)));
        }

        /// <summary>
        /// Sends an image <see cref="ContributionRequest" /> message to the server.
        /// </summary>
        /// <param name="conversationId">The Id of the conversation the Client wants to send the message to.</param>
        /// <param name="image">The image to add to a conversation</param>
        public void SendContribution(int conversationId, Image image)
        {
            connectionHandler.SendMessage(new ContributionRequest(new ImageContribution(ClientUserId, image, conversationId)));
        }

        /// <summary>
        /// Sends an <see cref="AvatarRequest" /> message to the server to change a user's avatar.
        /// </summary>
        /// <param name="avatar">The new image the user requests to have.</param>
        public void SendAvatarRequest(Image avatar)
        {
            connectionHandler.SendMessage(new AvatarRequest(ClientUserId, avatar));
        }

        /// <summary>
        /// Sends a <see cref="UserTypingRequest" /> message to the server to change the status of a current user's state of
        /// typing.
        /// </summary>
        /// <param name="participationId">The participation Id that holds reference to the user and conversation.</param>
        /// <param name="isTyping">Whether the user has started or finished typing.</param>
        public void SendUserTypingRequest(int participationId, bool isTyping)
        {
            userTypingThroughputLimiter.QueueMessageRequest(new UserTypingRequest(new UserTyping(isTyping, participationId)));
        }

        private void userTypingThroughputLimiter_DeferredSendLastMessage(object sender, UserTypingRequest e)
        {
            connectionHandler.SendMessage(e);
        }

        private void OnNewMessageReceived(object sender, MessageEventArgs e)
        {
            IMessage message = e.Message;
            IMessageHandler handler = null;
            try
            {
                handler = messageHandlerRegistry.MessageHandlersIndexedByMessageIdentifier[message.MessageIdentifier];

            }
            catch (KeyNotFoundException keyNotFoundException)
            {
                Log.Error("ClientService is not supposed to handle message with identifier: " + e.Message.MessageIdentifier, keyNotFoundException);
            }

            handler?.HandleMessage(message);
        }

        private void OnBootstrapCompleted(object sender, EventArgs e)
        {
            EventHandler bootstrapCompletedCopy = BootstrapCompleted;

            if (bootstrapCompletedCopy != null)
            {
                bootstrapCompletedCopy(this, EventArgs.Empty);
            }
        }
    }
}