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
            ParticipationSnapshot participationSnapshot = (ParticipationSnapshot) message;
            ParticipationSnapshotContext participationSnapshotContext = (ParticipationSnapshotContext) context;

            participationSnapshotContext.ParticipationRepository.AddParticipations(participationSnapshot.Participations);
        }
    }
}