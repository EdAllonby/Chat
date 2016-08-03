using SharedClasses;
using SharedClasses.Domain;
using SharedClasses.Message;

namespace ChatClient.Services.MessageHandler
{
    /// <summary>
    /// Handles a <see cref="EntityNotification{TEntity}" /> the Client received.
    /// </summary>
    internal sealed class UserNotificationHandler : MessageHandler<EntityNotification<User>>
    {
        public UserNotificationHandler(IServiceRegistry serviceRegistry) : base(serviceRegistry)
        {
        }

        protected override void HandleMessage(EntityNotification<User> message)
        {
            var userRepository = (IEntityRepository<User>) ServiceRegistry.GetService<RepositoryManager>().GetRepository<User>();

            switch (message.NotificationType)
            {
                case NotificationType.Create:
                    userRepository.AddEntity(message.Entity);
                    break;
            }
        }
    }
}