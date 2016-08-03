using System.Collections.Generic;
using System.Linq;
using SharedClasses;
using SharedClasses.Domain;
using SharedClasses.Message;

namespace Server.MessageHandler
{
    /// <summary>
    /// Handles a <see cref="ParticipationRequest" /> the Server received.
    /// </summary>
    internal sealed class ParticipationRequestHandler : MessageHandler<ParticipationRequest>
    {
        public ParticipationRequestHandler(IServiceRegistry serviceRegistry) : base(serviceRegistry)
        {
        }

        protected override void HandleMessage(ParticipationRequest message)
        {
            var participationRepository = (ParticipationRepository)ServiceRegistry.GetService<RepositoryManager>().GetRepository<Participation>();

            if (CheckUserCanEnterConversation(message, participationRepository))
            {
                var entityIdAllocatorFactory = ServiceRegistry.GetService<EntityIdAllocatorFactory>();
                AddUserToConversation(message, entityIdAllocatorFactory, participationRepository);
            }
        }

        private static bool CheckUserCanEnterConversation(ParticipationRequest participationRequest,
            ParticipationRepository participationRepository)
        {
            Participation newparticipation = participationRequest.Participation;

            List<Participation> currentParticipantsInConversation = participationRepository.GetParticipationsByConversationId(newparticipation.ConversationId);

            if (currentParticipantsInConversation.Any(participation => participation.UserId == newparticipation.UserId))
            {
                Log.WarnFormat($"User with id {participationRequest.Participation.UserId} cannot be added to conversation {participationRequest.Participation.ConversationId}, user already exists in this conversation.");

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