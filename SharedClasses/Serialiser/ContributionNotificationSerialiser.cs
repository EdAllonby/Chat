using System.Net.Sockets;
using SharedClasses.Message;

namespace SharedClasses.Serialiser
{
    /// <summary>
    /// Used to serialise and deserialise a <see cref="ContributionNotification" /> object
    /// Uses <see cref="ContributionSerialiser" /> for its underlying serialiser
    /// </summary>
    internal sealed class ContributionNotificationSerialiser : Serialiser<ContributionNotification>
    {
        private readonly ContributionSerialiser contributionSerialiser = new ContributionSerialiser();

        private readonly MessageIdentifierSerialiser messageIdentifierSerialiser = new MessageIdentifierSerialiser();

        protected override void Serialise(ContributionNotification contributionNotificationMessage, NetworkStream networkStream)
        {
            messageIdentifierSerialiser.SerialiseMessageIdentifier(MessageIdentifier.ContributionNotification, networkStream);
            contributionSerialiser.Serialise(contributionNotificationMessage.Contribution, networkStream);
        }

        public override IMessage Deserialise(NetworkStream networkStream)
        {
            var notification = new ContributionNotification(contributionSerialiser.Deserialise(networkStream));
            Log.InfoFormat("{0} message deserialised", notification.MessageIdentifier);
            return notification;
        }
    }
}