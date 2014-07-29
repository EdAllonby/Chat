using System.Collections.Generic;
using System.Linq;
using SharedClasses;
using SharedClasses.Domain;
using SharedClasses.Message;

namespace Server.MessageHandler
{
    /// <summary>
    /// Handles a <see cref="ConversationSnapshotRequest"/> the Server received.
    /// </summary>
    internal sealed class ConversationSnapshotRequestHandler : IMessageHandler
    {
        public void HandleMessage(IMessage message, IMessageContext context)
        {
            var conversationSnapshotRequest = (ConversationSnapshotRequest) message;
            var conversationSnapshotRequestContext = (ConversationSnapshotRequestContext) context;

            SendConversationSnapshot(conversationSnapshotRequest, conversationSnapshotRequestContext);
        }

        private static void SendConversationSnapshot(ConversationSnapshotRequest conversationSnapshotRequest,
            ConversationSnapshotRequestContext conversationSnapshotRequestContext)
        {
            IEnumerable<int> conversationIds =
                conversationSnapshotRequestContext.ParticipationRepository.GetAllConversationIdsByUserId(
                    conversationSnapshotRequest.UserId);

            IList<int> conversationEnumerable = conversationIds as IList<int> ?? conversationIds.ToList();

            List<Conversation> conversations =
                conversationEnumerable.Select(
                    conversationId =>
                        conversationSnapshotRequestContext.ConversationRepository.FindEntityById(conversationId)).ToList();

            var conversationSnapshot = new ConversationSnapshot(conversations);

            conversationSnapshotRequestContext.ClientHandlersIndexedByUserId[conversationSnapshotRequest.UserId].SendMessage(
                conversationSnapshot);
        }
    }
}