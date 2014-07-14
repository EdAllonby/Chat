using SharedClasses;
using SharedClasses.Domain;

namespace ChatClient.Services.MessageHandler
{
    internal sealed class ConnectionStatusNotificationContext : IMessageContext
    {
        private readonly UserRepository userRepository;

        public ConnectionStatusNotificationContext(UserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        public UserRepository UserRepository
        {
            get { return userRepository; }
        }
    }
}