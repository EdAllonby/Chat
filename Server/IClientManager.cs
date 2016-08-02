using System.Collections.Generic;
using SharedClasses;
using SharedClasses.Domain;
using SharedClasses.Message;

namespace Server
{
    internal interface IClientManager : IService
    {
        /// <summary>
        /// Sends an <see cref="IMessage" /> to all clients.
        /// </summary>
        /// <param name="message">The <see cref="IMessage" /> to send to all clients.</param>
        void SendMessageToClients(IMessage message);

        /// <summary>
        /// Sends an <see cref="IMessage" /> to selected clients.
        /// </summary>
        /// <param name="message">The <see cref="IMessage" /> to send to selected clients.</param>
        /// <param name="userIds">The users to send the <see cref="IMessage" /> to.</param>
        void SendMessageToClients(IMessage message, IEnumerable<int> userIds);

        /// <summary>
        /// Sends an <see cref="IMessage" /> to a client.
        /// </summary>
        /// <param name="message">The <see cref="IMessage" /> to send to a client.</param>
        /// <param name="userId">The client's <see cref="User" /> Id to send the <see cref="IMessage" /> to.</param>
        void SendMessageToClient(IMessage message, int userId);

        /// <summary>
        /// Add an <see cref="IClientHandler" />.
        /// </summary>
        /// <param name="userId">The client's <see cref="User" /> Id.</param>
        /// <param name="clientHandler">The <see cref="IClientHandler" /> to add.</param>
        void AddClientHandler(int userId, IClientHandler clientHandler);

        /// <summary>
        /// Removes a <see cref="ClientHandler" />.
        /// </summary>
        /// <param name="userId">The <see cref="ClientHandler" />'s associated <see cref="User" /> Id.</param>
        void RemoveClientHandler(int userId);

        /// <summary>
        /// Check to see if <see cref="ClientHandler" /> exists.
        /// </summary>
        /// <param name="userId">The <see cref="ClientHandler" />'s associated <see cref="User" /> Id.</param>
        /// <returns></returns>
        bool HasClientHandler(int userId);
    }
}