using System;
using SharedClasses.Domain;
using SharedClasses.Message;

namespace ChatClient.Services.MessageHandler
{
    /// <summary>
    /// Handles a <see cref="ParticipationSnapshot"/> the Client received.
    /// </summary>
    internal sealed class ParticipationSnapshotHandler : IClientMessageHandler
    {
        public void HandleMessage(IMessage message, IClientMessageContext context)
        {
            var participationSnapshot = (ParticipationSnapshot) message;

            IRepository<Participation> participationRepository = (IRepository<Participation>) context.RepositoryManager.GetRepository<Participation>();

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
            if (handler != null) handler(this, EventArgs.Empty);
        }
    }
}