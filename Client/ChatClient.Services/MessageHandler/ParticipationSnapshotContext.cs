using SharedClasses;
using SharedClasses.Domain;

namespace ChatClient.Services.MessageHandler
{
    /// <summary>
    /// Holds a reference to the necessary objects a <see cref="ParticipationSnapshotHandler"/> needs. 
    /// </summary>
    internal sealed class ParticipationSnapshotContext : IMessageContext
    {
        private readonly ParticipationRepository participationRepository;

        public ParticipationSnapshotContext(ParticipationRepository participationRepository)
        {
            this.participationRepository = participationRepository;
        }

        public ParticipationRepository ParticipationRepository
        {
            get { return participationRepository; }
        }
    }
}