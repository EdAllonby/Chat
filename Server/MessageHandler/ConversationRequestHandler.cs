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

            ParticipationRepository participationRepository = (ParticipationRepository) serviceRegistry.GetService<RepositoryManager>().GetRepository<Participation>();
            IRepository<Conversation> conversationRepository = (IRepository<Conversation>) serviceRegistry.GetService<RepositoryManager>().GetRepository<Conversation>();

            var newConversationRequest = (ConversationRequest) message;

            if (IsConversationValid(newConversationRequest, participationRepository))
            {
                CreateConversationEntity(newConversationRequest, conversationRepository, participationRepository, entityIdAllocatorFactory);
            }
        }

        private static void CreateConversationEntity(ConversationRequest conversationRequest, IRepository<Conversation> conversationRepository, IRepository<Participation> participationRepository, EntityIdAllocatorFactory entityIdAllocatorFactory)
        {
            int conversationId = entityIdAllocatorFactory.AllocateEntityId<Conversation>();

            var newConversation = new Conversation(conversationId);

            var participations = new List<Participation>();

            foreach (int userId in conversationRequest.UserIds)
            {
                int participationId = entityIdAllocatorFactory.AllocateEntityId<Participation>();
                participations.Add(new Participation(participationId, userId, conversationId));
            }

            participations.ForEach(participationRepository.AddEntity);

            conversationRepository.AddEntity(newConversation);
        }

        private static bool IsConversationValid(ConversationRequest conversationRequest, ParticipationRepository participationRepository)
        {
            // Check for no repeating users
            if (conversationRequest.UserIds.Count != conversationRequest.UserIds.Distinct().Count())
            {
                Log.Warn("Cannot make a conversation between two users of same id.");
                return false;
            }

            return !participationRepository.DoesConversationWithUsersExist(conversationRequest.UserIds);
        }
    }
}