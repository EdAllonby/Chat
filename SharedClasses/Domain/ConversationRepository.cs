using System.Collections.Generic;
using log4net;

namespace SharedClasses.Domain
{
    /// <summary>
    /// Holds a collection of <see cref="Conversation"/>s with basic CRUD operations.
    /// </summary>
    internal sealed class ConversationRepository : IEntityRepository<Conversation>
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (ConversationRepository));

        private readonly Dictionary<int, Conversation> conversationsIndexedById = new Dictionary<int, Conversation>();

        /// <summary>
        /// Adds a <see cref="Conversation"/> entity to the repository.
        /// </summary>
        /// <param name="conversation"><see cref="Conversation"/> entity to add.</param>
        public void AddEntity(Conversation conversation)
        {
            conversationsIndexedById[conversation.ConversationId] = conversation;
            Log.Debug("Conversation with Id " + conversation.ConversationId + " added to conversation repository");
        }

        /// <summary>
        /// Adds <see cref="Conversation"/> entities to the repository.
        /// </summary>
        /// <param name="conversations"><see cref="Conversation"/> entities to add to the repository.</param>
        public void AddEntities(IEnumerable<Conversation> conversations)
        {
            foreach (Conversation conversation in conversations)
            {
                conversationsIndexedById[conversation.ConversationId] = conversation;
                Log.Debug("Conversation with Id " + conversation.ConversationId + " added to conversation repository");
            }
        }

        /// <summary>
        /// Removes a <see cref="Conversation"/> entity from the repository.
        /// </summary>
        /// <param name="conversationId">The <see cref="Conversation"/> entity to remove from the repository.</param>
        public void RemoveEntity(int conversationId)
        {
            conversationsIndexedById.Remove(conversationId);
            Log.Debug("Conversation with Id " + conversationId + " removed from Conversation repository");
        }

        /// <summary>
        /// Retrieves a <see cref="Conversation"/> entity from the repository.
        /// </summary>
        /// <param name="conversationID">The <see cref="Conversation"/> entity ID to find.</param>
        /// <returns>The <see cref="Conversation"/> which matches the ID. If no <see cref="Conversation"/> is found, return null.</returns>
        public Conversation FindEntityByID(int conversationID)
        {
            return conversationsIndexedById.ContainsKey(conversationID) ? conversationsIndexedById[conversationID] : null;
        }

        /// <summary>
        /// Retrieves all <see cref="Conversation"/> entities from the repository.
        /// </summary>
        /// <returns>A collection of all <see cref="Conversation"/> entities in the repository.</returns>
        public IEnumerable<Conversation> GetAllEntities()
        {
            return conversationsIndexedById.Values;
        }
    }
}