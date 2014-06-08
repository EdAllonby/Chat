using System.Collections.Generic;
using SharedClasses;
using SharedClasses.Domain;
using SharedClasses.Message;

namespace ChatClient.Services
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
        /// Holds the repositories for the Client.
        /// </summary>
        RepositoryManager RepositoryManager { get; }

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
    }
}