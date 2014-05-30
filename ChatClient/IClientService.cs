using System.Collections.Generic;
using ChatClient.Services;
using SharedClasses;
using SharedClasses.Domain;
using SharedClasses.Message;

namespace ChatClient
{
    /// <summary>
    /// The interface used to create a Client <see cref="IService"/> to talk to a Server.
    /// </summary>
    public interface IClientService : IService
    {
        /// <summary>
        /// The Client's User Id.
        /// </summary>
        int ClientUserId { get; }

        /// <summary>
        /// Raises an event when a new user has entered the system.
        /// </summary>
        event UserListHandler NewUser;

        /// <summary>
        /// Raises an event when a new conversation has been received.
        /// </summary>
        event NewConversationHandler NewConversationNotification;

        /// <summary>
        /// Raises an event when a new contribution has been received.
        /// </summary>
        event NewContributionNotificationHandler NewContributionNotification;

        /// <summary>
        /// Connects the Client to the server using the parameters as connection details
        /// and gets the state of <see cref="ClientService"/> up to date with the user status'. 
        /// </summary>
        /// <param name="loginDetails">The details used to log in to the Chat Program.</param>
        LoginResult LogOn(LoginDetails loginDetails);

        /// <summary>
        /// Sends a <see cref="NewConversationRequest"/> message to the server.
        /// </summary>
        /// <param name="participantIds">The participants that are included in the conversation.</param>
        void SendConversationRequest(List<int> participantIds);

        /// <summary>
        /// Sends a <see cref="ContributionRequest"/> message to the server.
        /// </summary>
        /// <param name="conversationID">The ID of the conversation the Client wants to send the message to.</param>
        /// <param name="message">The content of the message.</param>
        void SendContributionRequest(int conversationID, string message);

        /// <summary>
        /// Returns all <see cref="User"/>s that the client knows of.
        /// </summary>
        /// <returns>A collection of all <see cref="User"/>s.</returns>
        IEnumerable<User> GetAllUsers();

        /// <summary>
        /// Returns all <see cref="Participation"/>s that the client knows of.
        /// </summary>
        /// <returns>A collection of all <see cref="Participation"/>s.</returns>
        IEnumerable<Participation> GetAllParticipations();

        /// <summary>
        /// Returns the <see cref="User"/> entity object that is specific to this client.
        /// </summary>
        /// <param name="userId">The Id that matches the <see cref="User"/>.</param>
        /// <returns>The <see cref="User"/> that matches the <see cref="User"/> Id.</returns>
        User GetUser(int userId);

        /// <summary>
        /// Gets the <see cref="Conversation"/> object that matches the <see cref="Conversation"/> Id.
        /// </summary>
        /// <param name="conversationId">The Id that matches the <see cref="Conversation"/>.</param>
        /// <returns>The <see cref="Conversation"/> that matches the <see cref="Conversation"/> Id.</returns>
        Conversation GetConversation(int conversationId);

        /// <summary>
        /// Checks whether a <see cref="Conversation"/> exists for a particular set of <see cref="User"/>s.
        /// </summary>
        /// <param name="participantIds">The set of <see cref="User"/> Ids to check for a conversation.</param>
        /// <returns>Whether a conversation exists for the set of <see cref="User"/>s.</returns>
        bool DoesConversationExist(IEnumerable<int> participantIds);

        /// <summary>
        /// Gets the <see cref="Conversation"/> Id for the particular set of <see cref="User"/>s.
        /// </summary>
        /// <param name="participantIds">The set of <see cref="User"/> Ids that belong to the conversation.</param>
        /// <returns>The <see cref="Conversation"/> Id.</returns>
        int GetConversationId(IEnumerable<int> participantIds);
    }
}