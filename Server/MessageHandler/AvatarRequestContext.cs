using SharedClasses;
using SharedClasses.Domain;

namespace Server.MessageHandler
{
    internal sealed class AvatarRequestContext : IMessageContext
    {
        private readonly UserRepository userRepository;
        private readonly EntityIdAllocatorFactory entityIdAllocatorFactory;

        public AvatarRequestContext(UserRepository userRepository, EntityIdAllocatorFactory entityIdAllocatorFactory)
        {
            this.entityIdAllocatorFactory = entityIdAllocatorFactory;
            this.userRepository = userRepository;
        }

        public UserRepository UserRepository
        {
            get { return userRepository; }
        }

        public EntityIdAllocatorFactory EntityIdAllocatorFactory
        {
            get { return entityIdAllocatorFactory; }
        }
    }
}