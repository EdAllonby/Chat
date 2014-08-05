using System;
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

            context.RepositoryManager.ParticipationRepository.AddParticipations(participationSnapshot.Participations);

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