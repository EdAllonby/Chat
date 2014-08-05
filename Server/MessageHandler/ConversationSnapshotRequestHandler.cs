using System.Collections.Generic;
using System.Linq;
using SharedClasses.Domain;
using SharedClasses.Message;

namespace Server.MessageHandler
{
    /// <summary>
    /// Handles a <see cref="ConversationSnapshotRequest"/> the Server received.
    /// </summary>
    internal sealed class ConversationSnapshotRequestHandler : IMessageHandler
    {
        public void HandleMessage(IMessage message, IServerMessageContext context)
        {
            var conversationSnapshotRequest = (ConversationSnapshotRequest) message;

            SendConversationSnapshot(conversationSnapshotRequest, context);
        }

        private static void SendConversationSnapshot(ConversationSnapshotRequest conversationSnapshotRequest,
            IServerMessageContext context)
        {
            IEnumerable<int> conversationIds =
                context.RepositoryManager.ParticipationRepository.GetAllConversationIdsByUserId(
                    conversationSnapshotRequest.UserId);

            IList<int> conversationEnumerable = conversationIds as IList<int> ?? conversationIds.ToList();

            List<Conversation> conversations =
                conversationEnumerable.Select(
                    conversationId =>
                        context.RepositoryManager.ConversationRepository.FindEntityById(conversationId)).ToList();

            var conversationSnapshot = new ConversationSnapshot(conversations);

            context.ClientManager.SendMessageToClient(conversationSnapshot, conversationSnapshotRequest.UserId);
        }
    }
}