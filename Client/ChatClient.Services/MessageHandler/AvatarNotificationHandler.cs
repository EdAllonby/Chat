using SharedClasses.Domain;
using SharedClasses.Message;

namespace ChatClient.Services.MessageHandler
{
    /// <summary>
    /// Handles a <see cref="AvatarNotification"/> the Client received.
    /// </summary>
    internal sealed class AvatarNotificationHandler : IClientMessageHandler
    {
        public void HandleMessage(IMessage message, IClientMessageContext context)
        {
            var avatarNotification = (AvatarNotification) message;

            UserRepository userRepository = context.RepositoryManager.UserRepository;

            userRepository.UpdateUserAvatar(avatarNotification.Avatar);
        }
    }
}