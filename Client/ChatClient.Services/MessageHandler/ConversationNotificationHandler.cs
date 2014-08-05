using SharedClasses.Domain;
using SharedClasses.Message;

namespace ChatClient.Services.MessageHandler
{
    /// <summary>
    /// Handles a <see cref="ConversationNotification"/> the Client received.
    /// </summary>
    internal sealed class ConversationNotificationHandler : IClientMessageHandler
    {
        public void HandleMessage(IMessage message, IClientMessageContext context)
        {
            var conversationNotification = (ConversationNotification) message;

            ConversationRepository conversationRepository = context.RepositoryManager.ConversationRepository;

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