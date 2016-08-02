using System.Collections.Generic;
using System.Linq;
using log4net;
using SharedClasses;
using SharedClasses.Domain;
using SharedClasses.Message;

namespace Server.MessageHandler
{
    /// <summary>
    /// Handles a <see cref="ConversationRequest"/> the Server received.
    /// </summary>
    internal sealed class ConversationRequestHandler : IMessageHandler
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (ConversationRequestHandler));

        public void HandleMessage(IMessage message, IServiceRegistry serviceRegistry)
        {
            var entityIdAllocatorFactory = serviceRegistry.GetService<EntityIdAllocatorFactory>();

            var participationRepository = (ParticipationRepository) serviceRegistry.GetService<RepositoryManager>().GetRepository<Participation>();
            var conversationRepository = (IEntityRepository<Conversation>) serviceRegistry.GetService<RepositoryManager>().GetRepository<Conversation>();

            var newConversationRequest = (ConversationRequest) message;

            if (IsConversationValid(newConversationRequest, participationRepository))
            {
                CreateConversationEntity(newConversationRequest, conversationRepository, participationRepository, entityIdAllocatorFactory);
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