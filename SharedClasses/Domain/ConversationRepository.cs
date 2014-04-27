using System.Collections.Generic;

namespace SharedClasses.Domain
{
    public class ConversationRepository
    {
        private readonly Dictionary<int, Conversation> conversationsIndexedById = new Dictionary<int, Conversation>();

        public void AddConversation(Conversation conversation)
        {
            conversationsIndexedById[conversation.ConversationId] = conversation;
        }

        public void RemoveConversation(Conversation conversation)
        {
            // Remove conversation
        }

        public Conversation FindConversationById(int conversationId)
        {
            return conversationsIndexedById.ContainsKey(conversationId) ? conversationsIndexedById[conversationId] : null;
        }
    }
}