using SharedClasses;
using SharedClasses.Domain;
using SharedClasses.Message;

namespace ChatClient.Services.MessageHandler
{
    /// <summary>
    /// Handles a <see cref="AvatarNotification"/> the Client received.
    /// </summary>
    internal sealed class AvatarNotificationHandler : IMessageHandler
    {
        public void HandleMessage(IMessage message, IMessageContext context)
        {
            AvatarNotification avatarNotification = (AvatarNotification) message;
            AvatarNotificationContext avatarNotificationContext = (AvatarNotificationContext) context;

            UserRepository userRepository = avatarNotificationContext.UserRepository;

            userRepository.UpdateUserAvatar(avatarNotification.Avatar);
        }
    }
}