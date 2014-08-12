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

            foreach (Participation participation in participationSnapshot.Participations)
            {
                context.RepositoryManager.ParticipationRepository.AddEntity(participation);
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