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
    internal sealed class NewConversationRequestHandler : IMessageHandler
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (NewConversationRequestHandler));

        public void HandleMessage(IMessage message, IMessageContext context)
        {
            var newConversationRequest = (ConversationRequest) message;
            var newConversationRequestContext = (NewConversationRequestContext) context;

            if (CheckConversationIsValid(newConversationRequest, newConversationRequestContext.ParticipationRepository))
            {
                CreateConversationEntity(newConversationRequest, newConversationRequestContext);
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

        private void CreateConversationEntity(ConversationRequest conversationRequest,
            NewConversationRequestContext newConversationRequestContext)
        {
            int conversationId = newConversationRequestContext.EntityIdAllocatorFactory.AllocateEntityId<Conversation>();

            var newConversation = new Conversation(conversationId);

            var participations = new List<Participation>();

            foreach (int userId in conversationRequest.UserIds)
            {
                int participationId = newConversationRequestContext.EntityIdAllocatorFactory.AllocateEntityId<Participation>();
                participations.Add(new Participation(participationId, userId, conversationId));
            }

            newConversationRequestContext.ParticipationRepository.AddParticipations(participations);

            newConversationRequestContext.ConversationRepository.AddConversation(newConversation);
        }
    }
}