using SharedClasses;
using SharedClasses.Domain;

namespace Server.MessageHandler
{
    /// <summary>
    /// Holds a reference to the necessary objects a <see cref="ParticipationRequestHandler"/> needs. 
    /// </summary>
    internal sealed class ParticipationRequestContext : IMessageContext
    {
        private readonly EntityIdAllocatorFactory entityIdAllocatorFactory;
        private readonly ParticipationRepository participationRepository;

        public ParticipationRequestContext(ParticipationRepository participationRepository,
            EntityIdAllocatorFactory entityIdAllocatorFactory)
        {
            this.participationRepository = participationRepository;
            this.entityIdAllocatorFactory = entityIdAllocatorFactory;
        }

        public ParticipationRepository ParticipationRepository
        {
            get { return participationRepository; }
        }

        public EntityIdAllocatorFactory EntityIdAllocatorFactory
        {
            get { return entityIdAllocatorFactory; }
        }
    }
}