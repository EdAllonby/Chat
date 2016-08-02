using SharedClasses.Domain;

namespace SharedClasses.Message
{
    public sealed class ConnectionStatusNotification : IMessage
    {
        public ConnectionStatusNotification(ConnectionStatus connectionStatus, NotificationType notificationType)
        {
            ConnectionStatus = connectionStatus;
            NotificationType = notificationType;
        }

        public ConnectionStatus ConnectionStatus { get; }

        public NotificationType NotificationType { get; }

        public MessageIdentifier MessageIdentifier
        {
            get { return MessageIdentifier.ConnectionStatusNotification; }
        }
    }
}