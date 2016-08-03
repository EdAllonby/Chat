using SharedClasses;
using SharedClasses.Domain;
using SharedClasses.Message;

namespace ChatClient.Services.MessageHandler
{
    /// <summary>
    /// Handles a <see cref="EntityNotification{TEntity}" /> the Client received.
    /// </summary>
    internal sealed class ConnectionStatusNotificationHandler : MessageHandler<EntityNotification<ConnectionStatus>>
    {
        public ConnectionStatusNotificationHandler(IServiceRegistry serviceRegistry) : base(serviceRegistry)
        {
        }

        protected override void HandleMessage(EntityNotification<ConnectionStatus> message)
        {
            var userRepository = (UserRepository) ServiceRegistry.GetService<RepositoryManager>().GetRepository<User>();

            userRepository.UpdateUserConnectionStatus(message.Entity);
        }
    }
}