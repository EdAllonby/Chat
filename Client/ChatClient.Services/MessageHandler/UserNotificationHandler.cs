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
        public void HandleMessage(IMessage message, IMessageContext context)
        {
            var userNotification = (UserNotification) message;
            var userNotificationContext = (UserNotificationContext) context;

            UserRepository userRepository = userNotificationContext.UserRepository;

            switch (userNotification.NotificationType)
            {
                case NotificationType.Create:
                    userRepository.AddUser(userNotification.User);
                    break;
            }
        }
    }
}