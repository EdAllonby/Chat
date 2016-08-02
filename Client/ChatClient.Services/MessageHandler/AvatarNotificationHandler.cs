using SharedClasses;
using SharedClasses.Domain;
using SharedClasses.Message;

namespace ChatClient.Services.MessageHandler
{
    /// <summary>
    /// Handles a <see cref="EntityNotification{TEntity}" /> the Client received.
    /// </summary>
    internal sealed class AvatarNotificationHandler : IMessageHandler
    {
        public void HandleMessage(IMessage message, IServiceRegistry serviceRegistry)
        {
            var avatarNotification = (EntityNotification<Avatar>) message;

            var userRepository = (UserRepository) serviceRegistry.GetService<RepositoryManager>().GetRepository<User>();

            userRepository.UpdateUserAvatar(avatarNotification.Entity);
        }
    }
}