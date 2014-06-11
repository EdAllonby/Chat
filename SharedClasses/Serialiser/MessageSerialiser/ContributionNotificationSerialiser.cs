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

        protected override void Serialise(ContributionNotification contributionNotificationMessage, NetworkStream networkStream)
        {
            messageIdentifierSerialiser.SerialiseMessageIdentifier(MessageIdentifier.ContributionNotification, networkStream);
            contributionSerialiser.Serialise(networkStream, contributionNotificationMessage.Contribution);
        }

        public override IMessage Deserialise(NetworkStream networkStream)
        {
            var notification = new ContributionNotification(contributionSerialiser.Deserialise(networkStream));
            Log.InfoFormat("{0} message deserialised", notification.MessageIdentifier);
            return notification;
        }
    }
}