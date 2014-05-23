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

        protected override void Serialise(ContributionNotification contributionNotificationMessage, NetworkStream stream)
        {
            messageIdentifierSerialiser.SerialiseMessageIdentifier(MessageNumber.ContributionNotification, stream);
            contributionSerialiser.Serialise(contributionNotificationMessage.Contribution, stream);
        }

        public override IMessage Deserialise(NetworkStream stream)
        {
            var notification = new ContributionNotification(contributionSerialiser.Deserialise(stream));
            Log.InfoFormat("{0} message deserialised", notification.Identifier);
            return notification;
        }
    }
}