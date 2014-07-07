using System.Collections.Generic;
using SharedClasses;
using SharedClasses.Domain;
using SharedClasses.Message;

namespace Server.MessageHandler
{
    /// <summary>
    /// Handles a <see cref="ParticipationSnapshotRequest"/> the Server received.
    /// </summary>
    internal sealed class ParticipationSnapshotRequestHandler : IMessageHandler
    {
        public void HandleMessage(IMessage message, IMessageContext context)
        {
            var participationSnapshotRequest = (ParticipationSnapshotRequest) message;
            var participationSnapshotRequestContext = (ParticipationSnapshotRequestContext) context;

            SendParticipationSnapshot(participationSnapshotRequest, participationSnapshotRequestContext);
        }

        private static void SendParticipationSnapshot(ParticipationSnapshotRequest participationSnapshotRequest,
            ParticipationSnapshotRequestContext participationSnapshotRequestContext)
        {
            var userParticipations = new List<Participation>();

            foreach (int conversationId in participationSnapshotRequestContext.ParticipationRepository
                .GetAllConversationIdsByUserId(
                    participationSnapshotRequest.UserId))
            {
                userParticipations.AddRange(participationSnapshotRequestContext.ParticipationRepository
                    .GetParticipationsByConversationId(conversationId));
            }

            var participationSnapshot = new ParticipationSnapshot(userParticipations);

            participationSnapshotRequestContext.ClientHandlersIndexedByUserId[participationSnapshotRequest.UserId]
                .SendMessage(participationSnapshot);
        }
    }
}