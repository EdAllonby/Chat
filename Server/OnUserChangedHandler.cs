using SharedClasses;
using SharedClasses.Domain;
using SharedClasses.Message;

namespace Server
{
    internal sealed class OnUserChangedHandler : OnEntityChangedHandler
    {
        private readonly IReadOnlyEntityRepository<User> userRepository;

        public OnUserChangedHandler(IServiceRegistry serviceRegistry)
            : base(serviceRegistry)
        {
            userRepository = serviceRegistry.GetService<RepositoryManager>().GetRepository<User>();

            userRepository.EntityAdded += OnUserAdded;
            userRepository.EntityUpdated += OnUserUpdated;
        }

        private void OnUserAdded(object sender, EntityChangedEventArgs<User> e)
        {
            var userNotification = new EntityNotification<User>(e.Entity, NotificationType.Create);

            ClientManager.SendMessageToClients(userNotification);
        }

        private void OnUserUpdated(object sender, EntityChangedEventArgs<User> e)
        {
            if (e.PreviousEntity.ConnectionStatus.UserConnectionStatus != e.Entity.ConnectionStatus.UserConnectionStatus)
            {
                OnUserConnectionUpdated(e.Entity);
            }
            if (!e.PreviousEntity.Avatar.Equals(e.Entity.Avatar))
            {
                OnUserAvatarUpdated(e.Entity);
            }
        }

        private void OnUserConnectionUpdated(User user)
        {
            var userNotification = new EntityNotification<ConnectionStatus>(user.ConnectionStatus, NotificationType.Update);

            ClientManager.SendMessageToClients(userNotification);
        }

        private void OnUserAvatarUpdated(User user)
        {
            var avatarNotification = new EntityNotification<Avatar>(user.Avatar, NotificationType.Update);

            ClientManager.SendMessageToClients(avatarNotification);
        }

        public override void StopOnMessageChangedHandling()
        {
            userRepository.EntityAdded -= OnUserAdded;
            userRepository.EntityUpdated -= OnUserUpdated;
        }
    }
}