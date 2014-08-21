using SharedClasses;
using SharedClasses.Domain;
using SharedClasses.Message;

namespace Server.MessageHandler
{
    /// <summary>
    /// Handles a <see cref="AvatarRequest"/> the Client received.
    /// </summary>
    internal sealed class AvatarRequestHandler : IMessageHandler
    {
        public void HandleMessage(IMessage message, IServiceRegistry serviceRegistry)
        {
            var avatarRequest = (AvatarRequest) message;
            var entityIdAllocatorFactory = serviceRegistry.GetService<EntityIdAllocatorFactory>();
            UserRepository userRepository = (UserRepository) serviceRegistry.GetService<RepositoryManager>().GetRepository<User>();

            var avatar = new Avatar(entityIdAllocatorFactory.AllocateEntityId<Avatar>(), avatarRequest.Avatar);
            userRepository.UpdateUserAvatar(avatar);
        }
    }
}