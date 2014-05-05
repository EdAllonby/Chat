using System.Collections.Generic;
using log4net;

namespace SharedClasses.Domain
{
    public sealed class ConversationRepository
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (ConversationRepository));

        private readonly Dictionary<int, Conversation> conversationsIndexedById = new Dictionary<int, Conversation>();

        public void AddConversation(Conversation conversation)
        {
            conversationsIndexedById[conversation.ConversationId] = conversation;
            Log.Debug("Conversation with Id " + conversation.ConversationId + " added to user repository");
        }

        public void RemoveConversation(int conversationId)
        {
            conversationsIndexedById.Remove(conversationId);
            Log.Debug("Conversation with Id " + conversationId + " removed from Conversation repository");
        }

        public Conversation FindConversationById(int conversationId)
        {
            return conversationsIndexedById.ContainsKey(conversationId) ? conversationsIndexedById[conversationId] : null;
        }

        public IEnumerable<Conversation> GetAllConversations()
        {
            return conversationsIndexedById.Values;
        }
    }
}