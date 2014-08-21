using SharedClasses.Domain;
using SharedClasses.Message;

namespace ChatClient.Services.MessageHandler
{
    /// <summary>
    /// Handles a <see cref="UserNotification"/> the Client received.
    /// </summary>
    internal sealed class UserNotificationHandler : IClientMessageHandler
    {
        public void HandleMessage(IMessage message, IClientMessageContext context)
        {
            var userNotification = (UserNotification) message;

            var userRepository = (IRepository<User>) context.RepositoryManager.GetRepository<User>();

            switch (userNotification.NotificationType)
            {
                case NotificationType.Create:
                    userRepository.AddEntity(userNotification.User);
                    break;
            }
        }
    }
}