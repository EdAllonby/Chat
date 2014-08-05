using System.Linq;
using log4net;
using SharedClasses.Domain;
using SharedClasses.Message;

namespace Server.MessageHandler
{
    /// <summary>
    /// Handles a <see cref="ParticipationRequest"/> the Server received.
    /// </summary>
    internal sealed class ParticipationRequestHandler : IMessageHandler
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (Server));

        public void HandleMessage(IMessage message, IServerMessageContext context)
        {
            var participationRequest = (ParticipationRequest) message;

            if (CheckUserCanEnterConversation(participationRequest, context.RepositoryManager.ParticipationRepository))
            {
                AddUserToConversation(participationRequest, context.EntityIdAllocatorFactory,
                    context.RepositoryManager.ParticipationRepository);
            }
        }

        private static bool CheckUserCanEnterConversation(ParticipationRequest participationRequest,
            ParticipationRepository participationRepository)
        {
            Participation newparticipation = participationRequest.Participation;

            if (participationRepository.GetParticipationsByConversationId(newparticipation.ConversationId)
                .Any(participation => participation.UserId == newparticipation.UserId))
            {
                Log.WarnFormat(
                    "User with id {0} cannot be added to conversation {1}, user already exists in this conversation.",
                    participationRequest.Participation.UserId, participationRequest.Participation.ConversationId);

                return false;
            }

            return true;
        }

        private static void AddUserToConversation(ParticipationRequest participationRequest,
            EntityIdAllocatorFactory entityIdAllocatorFactory,
            Repository<Participation> participationRepository)
        {
            int participationId = entityIdAllocatorFactory.AllocateEntityId<Participation>();

            var participation = new Participation(participationId, participationRequest.Participation.UserId,
                participationRequest.Participation.ConversationId);

            participationRepository.AddEntity(participation);
        }
    }
}