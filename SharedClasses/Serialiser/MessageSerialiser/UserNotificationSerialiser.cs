using System.Net.Sockets;
using SharedClasses.Domain;
using SharedClasses.Message;
using SharedClasses.Serialiser.EntitySerialiser;

namespace SharedClasses.Serialiser.MessageSerialiser
{
    /// <summary>
    /// Used to Serialise and Deserialise a <see cref="User" /> Domain object.
    /// </summary>
    internal sealed class UserNotificationSerialiser : Serialiser<UserNotification>
    {
        private readonly NotificationTypeSerialiser notificationTypeSerialiser = new NotificationTypeSerialiser();
        private readonly UserSerialiser userSerialiser = new UserSerialiser();

        protected override void Serialise(NetworkStream networkStream, UserNotification message)
        {
            notificationTypeSerialiser.Serialise(networkStream, message.NotificationType);
            userSerialiser.Serialise(networkStream, message.User);
            Log.InfoFormat("{0} serialised and sent to network stream", message);
        }

        public override IMessage Deserialise(NetworkStream networkStream)
        {
            NotificationType notificationType = notificationTypeSerialiser.Deserialise(networkStream);

            var userNotification = new UserNotification(userSerialiser.Deserialise(networkStream), notificationType);
            
            Log.InfoFormat("Network stream has received data and deserialised to a {0} object", userNotification.MessageIdentifier);
            return userNotification;
        }
    }
}