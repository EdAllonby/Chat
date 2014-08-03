using System;
using SharedClasses;
using SharedClasses.Message;

namespace ChatClient.Services.MessageHandler
{
    /// <summary>
    /// Handles a <see cref="ParticipationSnapshot"/> the Client received.
    /// </summary>
    internal sealed class ParticipationSnapshotHandler : IMessageHandler
    {
        public void HandleMessage(IMessage message, IMessageContext context)
        {
            var participationSnapshot = (ParticipationSnapshot) message;
            var participationSnapshotContext = (ParticipationSnapshotContext) context;

            participationSnapshotContext.ParticipationRepository.AddParticipations(participationSnapshot.Participations);

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