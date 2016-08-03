using SharedClasses;
using SharedClasses.Domain;
using SharedClasses.Message;

namespace ChatClient.Services.MessageHandler
{
    /// <summary>
    /// Handles a <see cref="EntityNotification{TEntity}" /> the Client received.
    /// </summary>
    internal sealed class ConversationNotificationHandler : MessageHandler<EntityNotification<Conversation>>
    {
        public ConversationNotificationHandler(IServiceRegistry serviceRegistry) : base(serviceRegistry)
        {
        }

        protected override void HandleMessage(EntityNotification<Conversation> message)
        {
            var conversationRepository = (ConversationRepository) ServiceRegistry.GetService<RepositoryManager>().GetRepository<Conversation>();

            switch (message.NotificationType)
            {
                case NotificationType.Create:
                    conversationRepository.AddEntity(message.Entity);
                    break;

                case NotificationType.Update:
                    conversationRepository.UpdateEntity(message.Entity);
                    break;
            }
        }
    }
}