using System.Collections.Generic;
using log4net;
using SharedClasses.Domain;
using SharedClasses.Message;

namespace Server
{
    /// <summary>
    /// Holds logic for a collection of <see cref="ClientHandler"/>s.
    /// </summary>
    internal sealed class ClientManager : IClientManager
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (ClientManager));

        private readonly Dictionary<int, ClientHandler> clientHandlersIndexedByUserId = new Dictionary<int, ClientHandler>();

        /// <summary>
        /// Sends an <see cref="IMessage"/> to all clients.
        /// </summary>
        /// <param name="message">The <see cref="IMessage"/> to send to all clients.</param>
        public void SendMessageToClients(IMessage message)
        {
            foreach (ClientHandler clientHandler in clientHandlersIndexedByUserId.Values)
            {
                clientHandler.SendMessage(message);
            }
        }

        /// <summary>
        /// Sends an <see cref="IMessage"/> to selected clients.
        /// </summary>
        /// <param name="message">The <see cref="IMessage"/> to send to selected clients.</param>
        /// <param name="userIds">The users to send the <see cref="IMessage"/> to.</param>
        public void SendMessageToClients(IMessage message, IEnumerable<int> userIds)
        {
            foreach (int userId in userIds)
            {
                clientHandlersIndexedByUserId[userId].SendMessage(message);
            }
        }

        /// <summary>
        /// Sends an <see cref="IMessage"/> to a client.
        /// </summary>
        /// <param name="message">The <see cref="IMessage"/> to send to a client.</param>
        /// <param name="userId">The client's <see cref="User"/> Id to send the <see cref="IMessage"/> to.</param>
        public void SendMessageToClient(IMessage message, int userId)
        {
            clientHandlersIndexedByUserId[userId].SendMessage(message);
        }

        /// <summary>
        /// Add a <see cref="ClientHandler"/>.
        /// </summary>
        /// <param name="userId">The client's <see cref="User"/> Id.</param>
        /// <param name="clientHandler">The <see cref="ClientHandler"/> to add.</param>
        public void AddClientHandler(int userId, ClientHandler clientHandler)
        {
            clientHandlersIndexedByUserId.Add(userId, clientHandler);
        }

        /// <summary>
        /// Removes a <see cref="ClientHandler"/>.
        /// </summary>
        /// <param name="userId">The <see cref="ClientHandler"/>'s associated <see cref="User"/> Id.</param>
        public void RemoveClientHandler(int userId)
        {
            clientHandlersIndexedByUserId.Remove(userId);
            Log.InfoFormat("User with id {0} Removed from ClientHandler.", userId);
        }
    }
}