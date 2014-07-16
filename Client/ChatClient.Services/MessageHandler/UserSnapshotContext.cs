using SharedClasses;
using SharedClasses.Domain;

namespace ChatClient.Services.MessageHandler
{
    /// <summary>
    /// Holds a reference to the necessary objects a <see cref="UserSnapshotHandler"/> needs. 
    /// </summary>
    internal sealed class UserSnapshotContext : IMessageContext
    {
        private readonly UserRepository userRepository;

        public UserSnapshotContext(UserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        public UserRepository UserRepository
        {
            get { return userRepository; }
        }
    }
}