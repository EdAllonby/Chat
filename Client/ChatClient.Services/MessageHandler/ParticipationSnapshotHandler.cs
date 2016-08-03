using System;
using SharedClasses;
using SharedClasses.Domain;
using SharedClasses.Message;

namespace ChatClient.Services.MessageHandler
{
    /// <summary>
    /// Handles a <see cref="EntitySnapshot{Participation}" /> the Client received.
    /// </summary>
    internal sealed class ParticipationSnapshotHandler : IMessageHandler
    {
        public void HandleMessage(IMessage message, IServiceRegistry serviceRegistry)
        {
            var participationSnapshot = (EntitySnapshot<Participation>) message;

            var participationRepository = (IEntityRepository<Participation>) serviceRegistry.GetService<RepositoryManager>().GetRepository<Participation>();

            foreach (Participation participation in participationSnapshot.Entities)
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