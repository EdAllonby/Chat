using SharedClasses;
using SharedClasses.Domain;
using SharedClasses.Message;

namespace ChatClient.Services.MessageHandler
{
    /// <summary>
    /// Handles a <see cref="UserNotification"/> the Client received.
    /// </summary>
    internal sealed class UserNotificationHandler : IMessageHandler
    {
        public void HandleMessage(IMessage message, IServiceRegistry serviceRegistry)
        {
            var userNotification = (UserNotification) message;

            var userRepository = (IEntityRepository<User>) serviceRegistry.GetService<RepositoryManager>().GetRepository<User>();

            switch (userNotification.NotificationType)
            {
                case NotificationType.Create:
                    userRepository.AddEntity(userNotification.User);
                    break;
            }
        }
    }
}