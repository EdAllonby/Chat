using SharedClasses;
using SharedClasses.Domain;
using SharedClasses.Message;

namespace ChatClient.Services.MessageHandler
{
    /// <summary>
    /// Handles a <see cref="EntityNotification{TEntity}" /> the Client received.
    /// </summary>
    internal sealed class ConnectionStatusNotificationHandler : IMessageHandler
    {
        public void HandleMessage(IMessage message, IServiceRegistry serviceRegistry)
        {
            var connectionStatusNotification = (EntityNotification<ConnectionStatus>) message;

            var userRepository = (UserRepository) serviceRegistry.GetService<RepositoryManager>().GetRepository<User>();

            userRepository.UpdateUserConnectionStatus(connectionStatusNotification.Entity);
        }
    }
}