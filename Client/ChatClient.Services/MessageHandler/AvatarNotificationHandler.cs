using SharedClasses;
using SharedClasses.Domain;
using SharedClasses.Message;

namespace ChatClient.Services.MessageHandler
{
    /// <summary>
    /// Handles a <see cref="EntityNotification{TEntity}" /> the Client received.
    /// </summary>
    internal sealed class AvatarNotificationHandler : MessageHandler<EntityNotification<Avatar>>
    {
        protected override void HandleMessage(EntityNotification<Avatar> message)
        {
            var userRepository = (UserRepository) ServiceRegistry.GetService<RepositoryManager>().GetRepository<User>();

            userRepository.UpdateUserAvatar(message.Entity);
        }

        public AvatarNotificationHandler(IServiceRegistry serviceRegistry) : base(serviceRegistry)
        {
        }
    }
}