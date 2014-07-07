using System.Collections.Generic;
using SharedClasses;
using SharedClasses.Message;

namespace ChatClient.Services
{
    /// <summary>
    /// The interface used to create a Client <see cref="IService"/> to talk to a Server.
    /// </summary>
    public interface IClientService : IService
    {
        /// <summary>
        /// This Client's unique User Id.
        /// </summary>
        int ClientUserId { get; }

        /// <summary>
        /// Holds the repositories for the Client.
        /// </summary>
        RepositoryManager RepositoryManager { get; }

        /// <summary>
        /// Connects the Client to the server using the parameters as connection details
        /// and gets the state of <see cref="ClientService"/> up to date with the user status'. 
        /// </summary>
        /// <param name="loginDetails">The details used to log in to the Chat Program.</param>
        LoginResult LogOn(LoginDetails loginDetails);

        /// <summary>
        /// Sends a <see cref="NewConversationRequest"/> message to the server.
        /// </summary>
        /// <param name="userIds">The participants that are included in the conversation.</param>
        void CreateConversation(List<int> userIds);

        /// <summary>
        /// Sends a <see cref="ParticipationRequest"/> message to the server to add a user to an existing conversation.
        /// </summary>
        /// <param name="userId">The participant that will be added to the conversation.</param>
        /// <param name="conversationId">The targetted conversation the participant will be added to.</param>
        void AddUserToConversation(int userId, int conversationId);

        /// <summary>
        /// Sends a <see cref="ContributionRequest"/> message to the server.
        /// </summary>
        /// <param name="conversationID">The ID of the conversation the Client wants to send the message to.</param>
        /// <param name="message">The content of the message.</param>
        void SendContribution(int conversationID, string message);
    }
}