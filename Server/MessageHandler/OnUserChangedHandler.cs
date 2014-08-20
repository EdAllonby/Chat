using SharedClasses;
using SharedClasses.Domain;
using SharedClasses.Message;

namespace Server.MessageHandler
{
    internal sealed class OnUserChangedHandler : OnEntityChangedHandler
    {
        public OnUserChangedHandler(IServiceRegistry serviceRegistry)
            : base(serviceRegistry)
        {
            RepositoryManager.UserRepository.EntityAdded += OnUserAdded;
            RepositoryManager.UserRepository.EntityUpdated += OnUserUpdated;
        }

        private void OnUserAdded(object sender, EntityChangedEventArgs<User> e)
        {
            var userNotification = new UserNotification(e.Entity, NotificationType.Create);

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
            var userNotification = new ConnectionStatusNotification(user.ConnectionStatus, NotificationType.Update);

            ClientManager.SendMessageToClients(userNotification);
        }

        private void OnUserAvatarUpdated(User user)
        {
            var avatarNotification = new AvatarNotification(user.Avatar, NotificationType.Update);

            ClientManager.SendMessageToClients(avatarNotification);
        }

        public override void StopOnMessageChangedHandling()
        {
            RepositoryManager.UserRepository.EntityAdded -= OnUserAdded;
            RepositoryManager.UserRepository.EntityUpdated -= OnUserUpdated;
        }
    }
}