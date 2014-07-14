using System.Net.Sockets;
using SharedClasses.Message;
using SharedClasses.Serialiser.EntitySerialiser;

namespace SharedClasses.Serialiser.MessageSerialiser
{
    /// <summary>
    /// Used to serialise and deserialise a <see cref="AvatarNotification" /> object.
    /// Uses an <see cref="AvatarSerialiser" /> for its underlying serialiser.
    /// </summary>
    internal sealed class AvatarNotificationSerialiser : Serialiser<AvatarNotification>
    {
        private readonly AvatarSerialiser avatarSerialiser = new AvatarSerialiser();
        private readonly NotificationTypeSerialiser notificationTypeSerialiser = new NotificationTypeSerialiser();

        protected override void Serialise(NetworkStream networkStream, AvatarNotification avatarNotification)
        {
            notificationTypeSerialiser.Serialise(networkStream, avatarNotification.NotificationType);
            avatarSerialiser.Serialise(networkStream, avatarNotification.Avatar);
        }

        public override IMessage Deserialise(NetworkStream networkStream)
        {
            NotificationType notificationType = notificationTypeSerialiser.Deserialise(networkStream);
            var avatarNotification = new AvatarNotification(avatarSerialiser.Deserialise(networkStream), notificationType);

            Log.InfoFormat("{0} message deserialised.", avatarNotification.MessageIdentifier);

            return avatarNotification;
        }
    }
}
