namespace Server
{
    internal sealed class ConversationIDGenerator
    {
        private int nextId;

        public int CreateConversationId()
        {
            return nextId++;
        }
    }
}