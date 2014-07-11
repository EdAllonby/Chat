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
            var conversationNotification = (ConversationNotification) message;
            var conversationNotificationContext = (ConversationNotificationContext) context;

            ConversationRepository conversationRepository = conversationNotificationContext.ConversationRepository;

            switch (conversationNotification.NotificationType)
            {
                case NotificationType.Create:
                    conversationRepository.AddConversation(conversationNotification.Conversation);
                    break;

                case NotificationType.Update:
                    conversationRepository.UpdateConversation(conversationNotification.Conversation);
                    break;
            }
        }
    }
}