using SharedClasses;
using SharedClasses.Domain;

namespace Server.MessageHandler
{
    /// <summary>
    /// Holds a reference to the necessary objects a <see cref="NewConversationRequestHandler"/> needs. 
    /// </summary>
    internal sealed class NewConversationRequestContext : IMessageContext
    {
        private readonly ConversationRepository conversationRepository;
        private readonly EntityGeneratorFactory entityGeneratorFactory;
        private readonly ParticipationRepository participationRepository;

        public NewConversationRequestContext(ParticipationRepository participationRepository,
            ConversationRepository conversationRepository,
            EntityGeneratorFactory entityGeneratorFactory)
        {
            this.participationRepository = participationRepository;
            this.entityGeneratorFactory = entityGeneratorFactory;
            this.conversationRepository = conversationRepository;
        }

        public ParticipationRepository ParticipationRepository
        {
            get { return participationRepository; }
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