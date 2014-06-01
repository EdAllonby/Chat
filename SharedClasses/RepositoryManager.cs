using SharedClasses.Domain;

namespace SharedClasses
{
    public sealed class RepositoryManager
    {
        private readonly ConversationRepository conversationRepository = new ConversationRepository();
        private readonly ParticipationRepository participationRepository = new ParticipationRepository();
        private readonly UserRepository userRepository = new UserRepository();

        public UserRepository UserRepository
        {
            get { return userRepository; }
        }

        public ConversationRepository ConversationRepository
        {
            get { return conversationRepository; }
        }

        public ParticipationRepository ParticipationRepository
        {
            get { return participationRepository; }
        }
    }
}