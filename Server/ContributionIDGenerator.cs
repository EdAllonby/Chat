namespace Server
{
    internal sealed class ContributionIDGenerator
    {
        private int nextID;

        public int CreateConversationId()
        {
            return nextID++;
        }
    }
}