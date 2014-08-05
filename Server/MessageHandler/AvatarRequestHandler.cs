using SharedClasses.Domain;
using SharedClasses.Message;

namespace Server.MessageHandler
{
    /// <summary>
    /// Handles a <see cref="AvatarRequest"/> the Client received.
    /// </summary>
    internal sealed class AvatarRequestHandler : IMessageHandler
    {
        public void HandleMessage(IMessage message, IServerMessageContext context)
        {
            var avatarRequest = (AvatarRequest) message;

            var avatar = new Avatar(context.EntityIdAllocatorFactory.AllocateEntityId<Avatar>(),
                avatarRequest.Avatar);

            context.RepositoryManager.UserRepository.UpdateUserAvatar(avatar);
        }
    }
}