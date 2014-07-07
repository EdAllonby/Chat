using System.Collections.Generic;
using System.Linq;
using log4net;
using SharedClasses;
using SharedClasses.Domain;
using SharedClasses.Message;

namespace Server.MessageHandler
{
    /// <summary>
    /// Handles a <see cref="NewConversationRequest"/> the Server received.
    /// </summary>
    internal sealed class NewConversationRequestHandler : IMessageHandler
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (NewConversationRequestHandler));

        public void HandleMessage(IMessage message, IMessageContext context)
        {
            var newConversationRequest = (NewConversationRequest) message;
            var newConversationRequestContext = (NewConversationRequestContext) context;

            if (CheckConversationIsValid(newConversationRequest, newConversationRequestContext.ParticipationRepository))
            {
                CreateConversationEntity(newConversationRequest, newConversationRequestContext);
            }
        }

        private static bool CheckConversationIsValid(NewConversationRequest newConversationRequest,
            ParticipationRepository participationRepository)
        {
            // Check for no repeating users
            if (newConversationRequest.UserIds.Count != newConversationRequest.UserIds.Distinct().Count())
            {
                Log.Warn("Cannot make a conversation between two users of same id");
                return false;
            }

            return !participationRepository.DoesConversationWithUsersExist(newConversationRequest.UserIds);
        }

        private void CreateConversationEntity(NewConversationRequest newConversationRequest,
            NewConversationRequestContext newConversationRequestContext)
        {
            int conversationId = newConversationRequestContext.EntityGeneratorFactory.GetEntityID<Conversation>();

            var newConversation = new Conversation(conversationId);

            var participations = new List<Participation>();

            foreach (int userId in newConversationRequest.UserIds)
            {
                int participationId = newConversationRequestContext.EntityGeneratorFactory.GetEntityID<Participation>();
                participations.Add(new Participation(participationId, userId, conversationId));
            }

            newConversationRequestContext.ParticipationRepository.AddParticipations(participations);

            newConversationRequestContext.ConversationRepository.AddConversation(newConversation);
        }
    }
}