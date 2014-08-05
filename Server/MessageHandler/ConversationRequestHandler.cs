using System.Collections.Generic;
using System.Linq;
using log4net;
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

        public void HandleMessage(IMessage message, IServerMessageContext context)
        {
            var newConversationRequest = (ConversationRequest) message;

            if (CheckConversationIsValid(newConversationRequest, context.RepositoryManager.ParticipationRepository))
            {
                CreateConversationEntity(newConversationRequest, context);
            }
        }

        private static bool CheckConversationIsValid(ConversationRequest conversationRequest,
            ParticipationRepository participationRepository)
        {
            // Check for no repeating users
            if (conversationRequest.UserIds.Count != conversationRequest.UserIds.Distinct().Count())
            {
                Log.Warn("Cannot make a conversation between two users of same id");
                return false;
            }

            return !participationRepository.DoesConversationWithUsersExist(conversationRequest.UserIds);
        }

        private static void CreateConversationEntity(ConversationRequest conversationRequest,
            IServerMessageContext context)
        {
            int conversationId = context.EntityIdAllocatorFactory.AllocateEntityId<Conversation>();

            var newConversation = new Conversation(conversationId);

            var participations = new List<Participation>();

            foreach (int userId in conversationRequest.UserIds)
            {
                int participationId = context.EntityIdAllocatorFactory.AllocateEntityId<Participation>();
                participations.Add(new Participation(participationId, userId, conversationId));
            }

            context.RepositoryManager.ParticipationRepository.AddEntities(participations);

            context.RepositoryManager.ConversationRepository.AddEntity(newConversation);
        }
    }
}