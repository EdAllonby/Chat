using SharedClasses;
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
            var userNotificationHandler = (UserNotification) message;
            var userNotificationContext = (UserNotificationContext) context;

            UpdateUserInRepository(userNotificationHandler, userNotificationContext);
        }

        private static void UpdateUserInRepository(UserNotification userNotification,
            UserNotificationContext userNotificationContext)
        {
            userNotificationContext.UserRepository.UpdateUser(userNotification.User);
        }
    }
}