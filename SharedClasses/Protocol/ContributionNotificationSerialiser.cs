using System.Net.Sockets;
using log4net;
using SharedClasses.Domain;

namespace SharedClasses.Protocol
{
    public class ContributionNotificationSerialiser : ISerialiser<ContributionNotification>
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (ContributionNotificationSerialiser));

        private readonly MessageIdentifierSerialiser messageIdentifierSerialiser = new MessageIdentifierSerialiser();
        private readonly ContributionSerialiser serialiser = new ContributionSerialiser();

        public void Serialise(ContributionNotification contributionNotificationMessage, NetworkStream stream)
        {
            messageIdentifierSerialiser.SerialiseMessageIdentifier(contributionNotificationMessage.Identifier, stream);

            Log.Debug("Waiting for a contribution notification message to serialise");
            serialiser.Serialise(contributionNotificationMessage.Contribution, stream);
            Log.Info("Contribution notification message serialised");
        }

        public void Serialise(IMessage contributionNotificationMessage, NetworkStream stream)
        {
            Serialise((ContributionNotification) contributionNotificationMessage, stream);
        }

        public IMessage Deserialise(NetworkStream networkStream)
        {
            Log.Debug("Waiting for a contribution notification message to deserialise");
            var notification = new ContributionNotification(serialiser.Deserialise(networkStream));
            Log.Info("Contribution notification message deserialised");
            return notification;
        }
    }
}