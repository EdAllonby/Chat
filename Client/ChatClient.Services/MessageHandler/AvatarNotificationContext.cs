using SharedClasses;
using SharedClasses.Domain;

namespace ChatClient.Services.MessageHandler
{
    internal sealed class AvatarNotificationContext : IMessageContext
    {
        private readonly UserRepository userRepository;

        public AvatarNotificationContext(UserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        public UserRepository UserRepository
        {
            get { return userRepository; }
        }
    }
}