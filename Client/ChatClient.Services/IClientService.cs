using System;
using System.Collections.Generic;
using System.Drawing;
using SharedClasses;
using SharedClasses.Message;

namespace ChatClient.Services
{
    /// <summary>
    /// The interface used to create a Client <see cref="IService" /> to talk to a Server.
    /// </summary>
    public interface IClientService : IService
    {
        /// <summary>
        /// This Client's unique User Id.
        /// </summary>
        int ClientUserId { get; }

        /// <summary>
        /// Signifies when the client has finished successfully bootstrapping
        /// </summary>
        event EventHandler BootstrapCompleted;

        /// <summary>
        /// Connects the Client to the server using the parameters as connection details
        /// and gets the state of <see cref="ClientService" /> up to date with the user status'.
        /// </summary>
        /// <param name="loginDetails">The details used to log in to the Chat Program.</param>
        LoginResult LogOn(LoginDetails loginDetails);

        /// <summary>
        /// Sends a <see cref="ConversationRequest" /> message to the server.
        /// </summary>
        /// <param name="userIds">The participants that are included in the conversation.</param>
        void CreateConversation(List<int> userIds);

        /// <summary>
        /// Sends a <see cref="ParticipationRequest" /> message to the server to add a user to an existing conversation.
        /// </summary>
        /// <param name="userId">The participant that will be added to the conversation.</param>
        /// <param name="conversationId">The targetted conversation the participant will be added to.</param>
        void AddUserToConversation(int userId, int conversationId);

        /// <summary>
        /// Sends an <see cref="AvatarRequest" /> message to the server to change a user's avatar.
        /// </summary>
        /// <param name="avatar">The new image the user requests to have.</param>
        void SendAvatarRequest(Image avatar);

        /// <summary>
        /// Sends a <see cref="UserTypingRequest" /> message to the server to change the status of a current user's state of
        /// typing.
        /// </summary>
        /// <param name="participationId">The participation Id that holds reference to the user and conversation.</param>
        /// <param name="isTyping">Whether the user has started or finished typing.</param>
        void SendUserTypingRequest(int participationId, bool isTyping);

        /// <summary>
        /// Sends a text <see cref="ContributionRequest" /> message to the server.
        /// </summary>
        /// <param name="conversationId">The Id of the conversation the Client wants to send the message to.</param>
        /// <param name="message">The content of the message.</param>
        void SendContribution(int conversationId, string message);

        /// <summary>
        /// Sends an image <see cref="ContributionRequest" /> message to the server.
        /// </summary>
        /// <param name="conversationId">The Id of the conversation the Client wants to send the message to.</param>
        /// <param name="image">The image to add to a conversation</param>
        void SendContribution(int conversationId, Image image);
    }
}