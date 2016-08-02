using SharedClasses.Domain;

namespace SharedClasses.Message
{
    public sealed class ConnectionStatusNotification : IMessage
    {
        private readonly ConnectionStatus connectionStatus;
        private readonly NotificationType notificationType;

        public ConnectionStatusNotification(ConnectionStatus connectionStatus, NotificationType notificationType)
        {
            this.connectionStatus = connectionStatus;
            this.notificationType = notificationType;
        }

        public ConnectionStatus ConnectionStatus
        {
            get { return connectionStatus; }
        }

        public NotificationType NotificationType
        {
            get { return notificationType; }
        }

        public MessageIdentifier MessageIdentifier
        {
            get { return MessageIdentifier.ConnectionStatusNotification; }
        }
    }
}