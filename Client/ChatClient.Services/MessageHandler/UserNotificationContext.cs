using SharedClasses;
using SharedClasses.Domain;

namespace ChatClient.Services.MessageHandler
{
    /// <summary>
    /// Holds a reference to the necessary objects a <see cref="UserNotificationHandler"/> needs. 
    /// </summary>
    internal sealed class UserNotificationContext : IMessageContext
    {
        private readonly UserRepository userRepository;

        public UserNotificationContext(UserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        public UserRepository UserRepository
        {
            get { return userRepository; }
        }
    }
}