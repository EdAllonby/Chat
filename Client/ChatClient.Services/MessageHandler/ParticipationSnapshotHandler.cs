using System;
using SharedClasses;
using SharedClasses.Domain;
using SharedClasses.Message;

namespace ChatClient.Services.MessageHandler
{
    /// <summary>
    /// Handles a <see cref="ParticipationSnapshot" /> the Client received.
    /// </summary>
    internal sealed class ParticipationSnapshotHandler : IMessageHandler
    {
        public void HandleMessage(IMessage message, IServiceRegistry serviceRegistry)
        {
            var participationSnapshot = (ParticipationSnapshot) message;

            var participationRepository = (IEntityRepository<Participation>) serviceRegistry.GetService<RepositoryManager>().GetRepository<Participation>();

            foreach (Participation participation in participationSnapshot.Participations)
            {
                participationRepository.AddEntity(participation);
            }

            OnParticipationBootstrapCompleted();
        }

        public event EventHandler ParticipationBootstrapCompleted;

        private void OnParticipationBootstrapCompleted()
        {
            EventHandler handler = ParticipationBootstrapCompleted;

            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }
    }
}