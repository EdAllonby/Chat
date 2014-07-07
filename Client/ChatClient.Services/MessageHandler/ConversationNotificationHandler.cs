using SharedClasses;
using SharedClasses.Domain;
using SharedClasses.Message;

namespace ChatClient.Services.MessageHandler
{
    /// <summary>
    /// Handles a <see cref="ConversationNotification"/> the Client received.
    /// </summary>
    internal sealed class ConversationNotificationHandler : IMessageHandler
    {
        public void HandleMessage(IMessage message, IMessageContext context)
        {
            var contributionNotification = (ConversationNotification) message;
            var contributionNotificationContext = (ConversationNotificationContext) context;

            AddConversationToRepository(contributionNotification, contributionNotificationContext);
        }

        private void AddConversationToRepository(ConversationNotification conversationNotification,
            ConversationNotificationContext contributionNotificationContext)
        {
            var conversation = new Conversation(conversationNotification.Conversation.ConversationId);
            contributionNotificationContext.ConversationRepository.AddConversation(conversation);
        }
    }
}