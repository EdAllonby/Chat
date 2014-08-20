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
        public void HandleMessage(IMessage message, IServiceRegistry serviceRegistry)
        {
            var participationSnapshotRequest = (ParticipationSnapshotRequest) message;

            ParticipationRepository participationRepository = serviceRegistry.GetService<RepositoryManager>().ParticipationRepository;
            var clientManager = serviceRegistry.GetService<IClientManager>();

            var userParticipations = new List<Participation>();

            foreach (int conversationId in participationRepository.GetAllConversationIdsByUserId(participationSnapshotRequest.UserId))
            {
                userParticipations.AddRange(participationRepository.GetParticipationsByConversationId(conversationId));
            }

            var participationSnapshot = new ParticipationSnapshot(userParticipations);

            clientManager.SendMessageToClient(participationSnapshot, participationSnapshotRequest.UserId);
        }
    }
}