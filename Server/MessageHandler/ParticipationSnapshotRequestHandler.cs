using System.Collections.Generic;
using SharedClasses;
using SharedClasses.Domain;
using SharedClasses.Message;

namespace Server.MessageHandler
{
    /// <summary>
    /// Handles a <see cref="EntitySnapshotRequest{Participation}" /> the Server received.
    /// </summary>
    internal sealed class ParticipationSnapshotRequestHandler : MessageHandler<EntitySnapshotRequest<Participation>>
    {
        public ParticipationSnapshotRequestHandler(IServiceRegistry serviceRegistry) : base(serviceRegistry)
        {
        }

        protected override void HandleMessage(EntitySnapshotRequest<Participation> message)
        {

            var participationRepository = (ParticipationRepository) ServiceRegistry.GetService<RepositoryManager>().GetRepository<Participation>();
            var clientManager = ServiceRegistry.GetService<IClientManager>();

            var userParticipations = new List<Participation>();

            foreach (int conversationId in participationRepository.GetAllConversationIdsByUserId(message.UserId))
            {
                userParticipations.AddRange(participationRepository.GetParticipationsByConversationId(conversationId));
            }

            var participationSnapshot = new EntitySnapshot<Participation>(userParticipations);

            clientManager.SendMessageToClient(participationSnapshot, message.UserId);
        }
    }
}