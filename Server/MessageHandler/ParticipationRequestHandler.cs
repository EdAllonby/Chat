using System.Collections.Generic;
using System.Linq;
using log4net;
using SharedClasses;
using SharedClasses.Domain;
using SharedClasses.Message;

namespace Server.MessageHandler
{
    /// <summary>
    /// Handles a <see cref="ParticipationRequest" /> the Server received.
    /// </summary>
    internal sealed class ParticipationRequestHandler : IMessageHandler
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(Server));

        public void HandleMessage(IMessage message, IServiceRegistry serviceRegistry)
        {
            var participationRequest = (ParticipationRequest) message;

            var participationRepository = (ParticipationRepository) serviceRegistry.GetService<RepositoryManager>().GetRepository<Participation>();

            if (CheckUserCanEnterConversation(participationRequest, participationRepository))
            {
                var entityIdAllocatorFactory = serviceRegistry.GetService<EntityIdAllocatorFactory>();
                AddUserToConversation(participationRequest, entityIdAllocatorFactory, participationRepository);
            }
        }

        private static bool CheckUserCanEnterConversation(ParticipationRequest participationRequest,
            ParticipationRepository participationRepository)
        {
            Participation newparticipation = participationRequest.Participation;

            List<Participation> currentParticipantsInConversation = participationRepository.GetParticipationsByConversationId(newparticipation.ConversationId);

            if (currentParticipantsInConversation.Any(participation => participation.UserId == newparticipation.UserId))
            {
                Log.WarnFormat(
                    $"User with id {participationRequest.Participation.UserId} cannot be added to conversation {participationRequest.Participation.ConversationId}, user already exists in this conversation.");

                return false;
            }

            return true;
        }

        private static void AddUserToConversation(ParticipationRequest participationRequest, EntityIdAllocatorFactory entityIdAllocatorFactory, IEntityRepository<Participation> participationRepository)
        {
            int participationId = entityIdAllocatorFactory.AllocateEntityId<Participation>();

            var participation = new Participation(participationId, participationRequest.Participation.UserId,
                participationRequest.Participation.ConversationId);

            participationRepository.AddEntity(participation);
        }
    }
}