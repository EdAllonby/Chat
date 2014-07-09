using System.Net.Sockets;
using SharedClasses.Message;
using SharedClasses.Serialiser.EntitySerialiser;

namespace SharedClasses.Serialiser.MessageSerialiser
{
    /// <summary>
    /// Used to serialise and deserialise a <see cref="ContributionNotification" /> object
    /// Uses <see cref="ContributionSerialiser" /> for its underlying serialiser
    /// </summary>
    internal sealed class ContributionNotificationSerialiser : Serialiser<ContributionNotification>
    {
        private readonly ContributionSerialiser contributionSerialiser = new ContributionSerialiser();
        private readonly MessageIdentifierSerialiser messageIdentifierSerialiser = new MessageIdentifierSerialiser();
        private readonly NotificationTypeSerialiser notificationTypeSerialiser = new NotificationTypeSerialiser();

        protected override void Serialise(ContributionNotification contributionNotificationMessage, NetworkStream networkStream)
        {
            messageIdentifierSerialiser.Serialise(networkStream, MessageIdentifier.ContributionNotification);
            notificationTypeSerialiser.Serialise(networkStream, contributionNotificationMessage.NotificationType);
            contributionSerialiser.Serialise(networkStream, contributionNotificationMessage.Contribution);
        }

        public override IMessage Deserialise(NetworkStream networkStream)
        {
            NotificationType notificationType = notificationTypeSerialiser.Deserialise(networkStream);
            var notification = new ContributionNotification(contributionSerialiser.Deserialise(networkStream), notificationType);

            Log.InfoFormat("{0} message deserialised", notification.MessageIdentifier);
            return notification;
        }
    }
}