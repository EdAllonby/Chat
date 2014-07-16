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
        }
    }
}