namespace SharedClasses
{
    public sealed class ConversationIDGenerator
    {
        private int nextId;

        public int CreateConversationId()
        {
            return nextId++;
        }
    }
}