using System.Net.Sockets;
using SharedClasses.Message;
using SharedClasses.Serialiser.EntitySerialiser;

namespace SharedClasses.Serialiser.MessageSerialiser
{
    public sealed class ConnectionStatusNotificationSerialiser : Serialiser<ConnectionStatusNotification>
    {
        private readonly ConnectionStatusSerialiser connectionStatusSerialiser = new ConnectionStatusSerialiser();
        private readonly NotificationTypeSerialiser notificationTypeSerialiser = new NotificationTypeSerialiser();

        protected override void Serialise(NetworkStream networkStream, ConnectionStatusNotification connectionStatusNotification)
        {
            notificationTypeSerialiser.Serialise(networkStream, connectionStatusNotification.NotificationType);
            connectionStatusSerialiser.Serialise(networkStream, connectionStatusNotification.ConnectionStatus);
        }

        public override IMessage Deserialise(NetworkStream networkStream)
        {
            NotificationType notificationType = notificationTypeSerialiser.Deserialise(networkStream);
            var connectionStatusNotification = new ConnectionStatusNotification(connectionStatusSerialiser.Deserialise(networkStream), notificationType);

            Log.InfoFormat("{0} message deserialised.", connectionStatusNotification.MessageIdentifier);

            return connectionStatusNotification;
        }
    }
}