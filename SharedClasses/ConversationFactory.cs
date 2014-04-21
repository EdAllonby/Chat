using SharedClasses.Domain;

namespace SharedClasses
{
    public sealed class ConversationFactory
    {
        public int NextID { get; private set; }

        public Conversation CreateConversation(User firstParticipant, User secondParticipant)
        {
            var conversation = new Conversation(NextID, firstParticipant, secondParticipant);
            NextID++;
            return conversation;
        }
    }
}