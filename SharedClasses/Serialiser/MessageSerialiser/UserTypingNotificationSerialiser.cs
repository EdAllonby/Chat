using System.Net.Sockets;
using SharedClasses.Message;
using SharedClasses.Serialiser.EntitySerialiser;

namespace SharedClasses.Serialiser.MessageSerialiser
{
    public sealed class UserTypingNotificationSerialiser : Serialiser<UserTypingNotification>
    {
        private readonly NotificationTypeSerialiser notificationTypeSerialiser = new NotificationTypeSerialiser();
        private readonly UserTypingSerialiser userTypingSerialiser = new UserTypingSerialiser();

        protected override void Serialise(NetworkStream networkStream, UserTypingNotification message)
        {
            notificationTypeSerialiser.Serialise(networkStream, message.NotificationType);
            userTypingSerialiser.Serialise(networkStream, message.UserTyping);
        }

        public override IMessage Deserialise(NetworkStream networkStream)
        {
            NotificationType notificationType = notificationTypeSerialiser.Deserialise(networkStream);

            var participationNotification = new UserTypingNotification(userTypingSerialiser.Deserialise(networkStream), notificationType);

            Log.InfoFormat("{0} message deserialised", participationNotification.MessageIdentifier);
            return participationNotification;
        }
    }
}