using System.Collections.Generic;
using log4net;

namespace SharedClasses.Domain
{
    internal sealed class ConversationRepository : IEntityRepository<Conversation>
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (ConversationRepository));

        private readonly Dictionary<int, Conversation> conversationsIndexedById = new Dictionary<int, Conversation>();

        public void AddEntity(Conversation conversation)
        {
            conversationsIndexedById[conversation.ConversationId] = conversation;
            Log.Debug("Conversation with Id " + conversation.ConversationId + " added to user repository");
        }

        public void AddEntities(IEnumerable<Conversation> conversations)
        {
            foreach (Conversation conversation in conversations)
            {
                conversationsIndexedById[conversation.ConversationId] = conversation;
                Log.Debug("Conversation with Id " + conversation.ConversationId + " added to conversation repository");
            }
        }

        public void RemoveEntity(int conversationId)
        {
            conversationsIndexedById.Remove(conversationId);
            Log.Debug("Conversation with Id " + conversationId + " removed from Conversation repository");
        }

        public Conversation FindEntityByID(int conversationID)
        {
            return conversationsIndexedById.ContainsKey(conversationID) ? conversationsIndexedById[conversationID] : null;
        }

        public IEnumerable<Conversation> GetAllEntities()
        {
            return conversationsIndexedById.Values;
        }
    }
}