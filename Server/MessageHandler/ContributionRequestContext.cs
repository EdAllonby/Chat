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
        private readonly EntityGeneratorFactory entityGeneratorFactory;

        public ContributionRequestContext(EntityGeneratorFactory entityGeneratorFactory,
            ConversationRepository conversationRepository)
        {
            this.entityGeneratorFactory = entityGeneratorFactory;
            this.conversationRepository = conversationRepository;
        }

        public EntityGeneratorFactory EntityGeneratorFactory
        {
            get { return entityGeneratorFactory; }
        }

        public ConversationRepository ConversationRepository
        {
            get { return conversationRepository; }
        }
    }
}