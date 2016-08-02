using System.Net.Sockets;
using SharedClasses.Message;
using SharedClasses.Serialiser.EntitySerialiser;

namespace SharedClasses.Serialiser.MessageSerialiser
{
    internal sealed class ParticipationNotificationSerialiser : Serialiser<ParticipationNotification>
    {
        private readonly NotificationTypeSerialiser notificationTypeSerialiser = new NotificationTypeSerialiser();
        private readonly ParticipationSerialiser participationSerialiser = new ParticipationSerialiser();

        protected override void Serialise(NetworkStream networkStream, ParticipationNotification message)
        {
            notificationTypeSerialiser.Serialise(networkStream, message.NotificationType);
            participationSerialiser.Serialise(networkStream, message.Participation);
        }

        public override IMessage Deserialise(NetworkStream networkStream)
        {
            NotificationType notificationType = notificationTypeSerialiser.Deserialise(networkStream);

            var participationNotification = new ParticipationNotification(participationSerialiser.Deserialise(networkStream), notificationType);

            Log.InfoFormat("{0} message deserialised", participationNotification.MessageIdentifier);
            return participationNotification;
        }
    }
}