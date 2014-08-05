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

            UserRepository userRepository = context.RepositoryManager.UserRepository;

            switch (userNotification.NotificationType)
            {
                case NotificationType.Create:
                    userRepository.AddEntity(userNotification.User);
                    break;
            }
        }
    }
}