using SharedClasses;
using SharedClasses.Domain;
using SharedClasses.Message;

namespace ChatClient.Services.MessageHandler
{
    /// <summary>
    /// Handles a <see cref="ConversationNotification" /> the Client received.
    /// </summary>
    internal sealed class ConversationNotificationHandler : IMessageHandler
    {
        public void HandleMessage(IMessage message, IServiceRegistry serviceRegistry)
        {
            var conversationNotification = (ConversationNotification) message;

            var conversationRepository = (ConversationRepository) serviceRegistry.GetService<RepositoryManager>().GetRepository<Conversation>();

            switch (conversationNotification.NotificationType)
            {
                case NotificationType.Create:
                    conversationRepository.AddEntity(conversationNotification.Conversation);
                    break;

                case NotificationType.Update:
                    conversationRepository.UpdateEntity(conversationNotification.Conversation);
                    break;
            }
        }
    }
}