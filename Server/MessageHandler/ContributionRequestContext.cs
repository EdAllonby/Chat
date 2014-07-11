using SharedClasses;
using SharedClasses.Domain;

namespace Server.MessageHandler
{
    /// <summary>
    /// Holds a reference to the necessary objects a <see cref="ContributionRequestHandler"/> needs. 
    /// </summary>
    internal sealed class ContributionRequestContext : IMessageContext
    {
        private readonly ConversationRepository conversationRepository;
        private readonly EntityIdAllocatorFactory entityIdAllocatorFactory;

        public ContributionRequestContext(EntityIdAllocatorFactory entityIdAllocatorFactory,
            ConversationRepository conversationRepository)
        {
            this.entityIdAllocatorFactory = entityIdAllocatorFactory;
            this.conversationRepository = conversationRepository;
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