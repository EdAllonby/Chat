using SharedClasses;
using SharedClasses.Domain;

namespace ChatClient.Services.MessageHandler
{
    /// <summary>
    /// Holds a reference to the necessary objects a <see cref="ParticipationNotificationHandler"/> needs. 
    /// </summary>
    internal sealed class ParticipationNotificationContext : IMessageContext
    {
        private readonly ParticipationRepository participationRepository;

        public ParticipationNotificationContext(ParticipationRepository participationRepository)
        {
            this.participationRepository = participationRepository;
        }

        public ParticipationRepository ParticipationRepository
        {
            get { return participationRepository; }
        }
    }
}