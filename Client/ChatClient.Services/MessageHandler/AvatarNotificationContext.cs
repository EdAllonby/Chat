using SharedClasses;
using SharedClasses.Domain;

namespace ChatClient.Services.MessageHandler
{
    /// <summary>
    /// Holds a reference to the necessary objects a <see cref="AvatarNotificationHandler"/> needs. 
    /// </summary>
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