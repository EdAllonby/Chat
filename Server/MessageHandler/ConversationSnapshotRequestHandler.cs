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
        public void HandleMessage(IMessage message, IServiceRegistry serviceRegistry)
        {
            var conversationSnapshotRequest = (ConversationSnapshotRequest) message;
            var participationRepository = (ParticipationRepository) serviceRegistry.GetService<RepositoryManager>().GetRepository<Participation>();
            IReadOnlyRepository<Conversation> conversationRepository = serviceRegistry.GetService<RepositoryManager>().GetRepository<Conversation>();
            var clientManager = serviceRegistry.GetService<IClientManager>();

            IEnumerable<int> conversationIds = participationRepository.GetAllConversationIdsByUserId(conversationSnapshotRequest.UserId);

            List<Conversation> conversations = conversationIds.Select(conversationRepository.FindEntityById).ToList();

            var conversationSnapshot = new ConversationSnapshot(conversations);

            clientManager.SendMessageToClient(conversationSnapshot, conversationSnapshotRequest.UserId);
        }
    }
}