using SharedClasses;
using SharedClasses.Domain;

namespace Server.MessageHandler
{
    /// <summary>
    /// Holds a reference to the necessary objects a <see cref="ParticipationRequestHandler"/> needs. 
    /// </summary>
    internal sealed class ParticipationRequestContext : IMessageContext
    {
        private readonly EntityGeneratorFactory entityGeneratorFactory;
        private readonly ParticipationRepository participationRepository;

        public ParticipationRequestContext(ParticipationRepository participationRepository,
            EntityGeneratorFactory entityGeneratorFactory)
        {
            this.participationRepository = participationRepository;
            this.entityGeneratorFactory = entityGeneratorFactory;
        }

        public ParticipationRepository ParticipationRepository
        {
            get { return participationRepository; }
        }

        public EntityGeneratorFactory EntityGeneratorFactory
        {
            get { return entityGeneratorFactory; }
        }
    }
}