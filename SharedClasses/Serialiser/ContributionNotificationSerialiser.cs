using System.Net.Sockets;
using log4net;
using SharedClasses.Message;

namespace SharedClasses.Serialiser
{
    /// <summary>
    /// Used to serialise and deserialise a <see cref="ContributionNotification" /> object
    /// Uses <see cref="ContributionSerialiser" /> for its underlying serialiser
    /// </summary>
    internal sealed class ContributionNotificationSerialiser : ISerialiser<ContributionNotification>
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (ContributionNotificationSerialiser));

        private readonly ContributionSerialiser contributionSerialiser = new ContributionSerialiser();

        private readonly MessageIdentifierSerialiser messageIdentifierSerialiser = new MessageIdentifierSerialiser();

        public void Serialise(ContributionNotification contributionNotificationMessage, NetworkStream stream)
        {
            messageIdentifierSerialiser.SerialiseMessageIdentifier(MessageNumber.ContributionNotification, stream);
            contributionSerialiser.Serialise(contributionNotificationMessage.Contribution, stream);
        }

        public void Serialise(IMessage contributionNotificationMessage, NetworkStream stream)
        {
            Serialise((ContributionNotification) contributionNotificationMessage, stream);
        }

        public IMessage Deserialise(NetworkStream networkStream)
        {
            var notification = new ContributionNotification(contributionSerialiser.Deserialise(networkStream));
            Log.InfoFormat("{0} message deserialised", notification.Identifier);
            return notification;
        }
    }
}