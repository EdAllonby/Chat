using System.Collections.Generic;
using System.Linq;
using SharedClasses;
using SharedClasses.Domain;
using SharedClasses.Message;

namespace Server.MessageHandler
{
    /// <summary>
    /// Handles a <see cref="ConversationRequest" /> the Server received.
    /// </summary>
    internal sealed class ConversationRequestHandler : MessageHandler<ConversationRequest>
    {
        public ConversationRequestHandler(IServiceRegistry serviceRegistry) : base(serviceRegistry)
        {
        }

        protected override void HandleMessage(ConversationRequest message)
        {
            var entityIdAllocatorFactory = ServiceRegistry.GetService<EntityIdAllocatorFactory>();

            var participationRepository = (ParticipationRepository)ServiceRegistry.GetService<RepositoryManager>().GetRepository<Participation>();
            var conversationRepository = (IEntityRepository<Conversation>)ServiceRegistry.GetService<RepositoryManager>().GetRepository<Conversation>();

            if (IsConversationValid(message, participationRepository))
            {
                CreateConversationEntity(message, conversationRepository, participationRepository, entityIdAllocatorFactory);
            }
        }

        private static void CreateConversationEntity(ConversationRequest conversationRequest, IEntityRepository<Conversation> conversationRepository, IEntityRepository<Participation> participationRepository, EntityIdAllocatorFactory entityIdAllocatorFactory)
        {
            var newConversation = new Conversation(entityIdAllocatorFactory.AllocateEntityId<Conversation>());

            var conversationParticipants = new List<Participation>();

            foreach (int userId in conversationRequest.UserIds)
            {
                int participationId = entityIdAllocatorFactory.AllocateEntityId<Participation>();
                conversationParticipants.Add(new Participation(participationId, userId, newConversation.Id));
            }

            conversationParticipants.ForEach(participationRepository.AddEntity);

            conversationRepository.AddEntity(newConversation);
        }

        private static bool IsConversationValid(ConversationRequest conversationRequest, ParticipationRepository participationRepository)
        {
            if (conversationRequest.UserIds.Count != conversationRequest.UserIds.Distinct().Count())
            {
                Log.Warn("Cannot make a conversation between two users of same id.");
                return false;
            }

            return !participationRepository.DoesConversationWithUsersExist(conversationRequest.UserIds);
        }
    }
}