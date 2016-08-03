using SharedClasses;
using SharedClasses.Domain;
using SharedClasses.Message;

namespace Server.MessageHandler
{
    /// <summary>
    /// Handles a <see cref="AvatarRequest" /> the Client received.
    /// </summary>
    internal sealed class AvatarRequestHandler : MessageHandler<AvatarRequest>
    {
        public AvatarRequestHandler(IServiceRegistry serviceRegistry) : base(serviceRegistry)
        {
        }

        protected override void HandleMessage(AvatarRequest message)
        {
            var entityIdAllocatorFactory = ServiceRegistry.GetService<EntityIdAllocatorFactory>();
            var userRepository = (UserRepository) ServiceRegistry.GetService<RepositoryManager>().GetRepository<User>();

            var avatar = new Avatar(entityIdAllocatorFactory.AllocateEntityId<Avatar>(), message.Avatar);
            userRepository.UpdateUserAvatar(avatar);
        }
    }
}