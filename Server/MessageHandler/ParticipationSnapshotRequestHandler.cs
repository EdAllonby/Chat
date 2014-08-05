using System.Collections.Generic;
using SharedClasses.Domain;
using SharedClasses.Message;

namespace Server.MessageHandler
{
    /// <summary>
    /// Handles a <see cref="ParticipationSnapshotRequest"/> the Server received.
    /// </summary>
    internal sealed class ParticipationSnapshotRequestHandler : IMessageHandler
    {
        public void HandleMessage(IMessage message, IServerMessageContext context)
        {
            var participationSnapshotRequest = (ParticipationSnapshotRequest) message;

            SendParticipationSnapshot(participationSnapshotRequest, context);
        }

        private static void SendParticipationSnapshot(ParticipationSnapshotRequest participationSnapshotRequest,
            IServerMessageContext context)
        {
            var userParticipations = new List<Participation>();

            foreach (int conversationId in context.RepositoryManager.ParticipationRepository
                .GetAllConversationIdsByUserId(
                    participationSnapshotRequest.UserId))
            {
                userParticipations.AddRange(context.RepositoryManager.ParticipationRepository
                    .GetParticipationsByConversationId(conversationId));
            }

            var participationSnapshot = new ParticipationSnapshot(userParticipations);

            context.ClientManager.SendMessageToClient(participationSnapshot, participationSnapshotRequest.UserId);
        }
    }
}