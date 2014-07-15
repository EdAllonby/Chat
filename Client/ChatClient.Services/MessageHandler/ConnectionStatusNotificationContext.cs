using SharedClasses;
using SharedClasses.Domain;

namespace ChatClient.Services.MessageHandler
{
    /// <summary>
    /// Holds a reference to the necessary objects a <see cref="ConnectionStatusNotificationHandler"/> needs. 
    /// </summary>
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