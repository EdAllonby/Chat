using SharedClasses;
using SharedClasses.Domain;

namespace Server.MessageHandler
{
    /// <summary>
    /// Holds a reference to the necessary objects a <see cref="ConversationRequestHandler"/> needs. 
    /// </summary>
    internal sealed class ConversationRequestContext : IMessageContext
    {
        private readonly ConversationRepository conversationRepository;
        private readonly EntityIdAllocatorFactory entityIdAllocatorFactory;
        private readonly ParticipationRepository participationRepository;

        public ConversationRequestContext(ParticipationRepository participationRepository,
            ConversationRepository conversationRepository,
            EntityIdAllocatorFactory entityIdAllocatorFactory)
        {
            this.participationRepository = participationRepository;
            this.entityIdAllocatorFactory = entityIdAllocatorFactory;
            this.conversationRepository = conversationRepository;
        }

        public ParticipationRepository ParticipationRepository
        {
            get { return participationRepository; }
        }

        public EntityIdAllocatorFactory EntityIdAllocatorFactory
        {
            get { return entityIdAllocatorFactory; }
        }

        public ConversationRepository ConversationRepository
        {
            get { return conversationRepository; }
        }
    }
}