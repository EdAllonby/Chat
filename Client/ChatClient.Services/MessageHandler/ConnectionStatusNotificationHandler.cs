using SharedClasses;
using SharedClasses.Domain;
using SharedClasses.Message;

namespace ChatClient.Services.MessageHandler
{
    /// <summary>
    /// Handles a <see cref="ConnectionStatusNotification"/> the Client received.
    /// </summary>
    internal sealed class ConnectionStatusNotificationHandler : IMessageHandler
    {
        public void HandleMessage(IMessage message, IMessageContext context)
        {
            ConnectionStatusNotification connectionStatusNotification = (ConnectionStatusNotification) message;
            ConnectionStatusNotificationContext connectionStatusNotificationContext = (ConnectionStatusNotificationContext) context;

            UserRepository userRepository = connectionStatusNotificationContext.UserRepository;

            userRepository.UpdateUserConnection(connectionStatusNotification.ConnectionStatus);
        }
    }
}