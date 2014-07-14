using SharedClasses;
using SharedClasses.Domain;
using SharedClasses.Message;

namespace Server.MessageHandler
{
    internal sealed class AvatarRequestHandler : IMessageHandler
    {
        public void HandleMessage(IMessage message, IMessageContext context)
        {
            AvatarRequest avatarRequest = (AvatarRequest) message;
            AvatarRequestContext avatarRequestContext = (AvatarRequestContext) context;

            Avatar avatar = new Avatar(avatarRequestContext.EntityIdAllocatorFactory.AllocateEntityId<Avatar>(),
                                       avatarRequest.Avatar);

            avatarRequestContext.UserRepository.UpdateUserAvatar(avatar);
        }
    }
}