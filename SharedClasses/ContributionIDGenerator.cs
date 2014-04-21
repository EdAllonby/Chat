using SharedClasses.Domain;

namespace SharedClasses
{
    public sealed class ContributionIDGenerator
    {
        public int NextID { get; private set; }

        public Contribution CreateConversation(User contributor, string message, Conversation conversation)
        {
            var contribution = new Contribution(NextID, contributor, message, conversation);
            NextID++;
            return contribution;
        }
    }
}