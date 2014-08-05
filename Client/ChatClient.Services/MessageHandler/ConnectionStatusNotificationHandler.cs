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

            UserRepository userRepository = context.RepositoryManager.UserRepository;

            userRepository.UpdateUserConnectionStatus(connectionStatusNotification.ConnectionStatus);
        }
    }
}