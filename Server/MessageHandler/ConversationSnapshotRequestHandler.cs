using System.Collections.Generic;
using System.Linq;
using SharedClasses;
using SharedClasses.Domain;
using SharedClasses.Message;

namespace Server.MessageHandler
{
    /// <summary>
    /// Handles a <see cref="EntitySnapshotRequest{Conversation}" /> the Server received.
    /// </summary>
    internal sealed class ConversationSnapshotRequestHandler : MessageHandler<EntitySnapshotRequest<Conversation>>
    {
        public ConversationSnapshotRequestHandler(IServiceRegistry serviceRegistry) : base(serviceRegistry)
        {
        }

        protected override void HandleMessage(EntitySnapshotRequest<Conversation> message)
        {
            var participationRepository = (ParticipationRepository) ServiceRegistry.GetService<RepositoryManager>().GetRepository<Participation>();
            IReadOnlyEntityRepository<Conversation> conversationRepository = ServiceRegistry.GetService<RepositoryManager>().GetRepository<Conversation>();
            var clientManager = ServiceRegistry.GetService<IClientManager>();

            IEnumerable<int> conversationIds = participationRepository.GetAllConversationIdsByUserId(message.UserId);

            List<Conversation> conversations = conversationIds.Select(conversationRepository.FindEntityById).ToList();

            var conversationSnapshot = new EntitySnapshot<Conversation>(conversations);

            clientManager.SendMessageToClient(conversationSnapshot, message.UserId);
        }
    }
}