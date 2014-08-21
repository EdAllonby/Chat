using SharedClasses.Domain;
using SharedClasses.Message;

namespace ChatClient.Services.MessageHandler
{
    /// <summary>
    /// Handles a <see cref="ConnectionStatusNotification"/> the Client received.
    /// </summary>
    internal sealed class ConnectionStatusNotificationHandler : IClientMessageHandler
    {
        public void HandleMessage(IMessage message, IClientMessageContext context)
        {
            var connectionStatusNotification = (ConnectionStatusNotification) message;

            var userRepository = (UserRepository) context.RepositoryManager.GetRepository<User>();

            userRepository.UpdateUserConnectionStatus(connectionStatusNotification.ConnectionStatus);
        }
    }
}