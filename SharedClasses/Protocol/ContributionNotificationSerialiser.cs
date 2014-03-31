using System.Net.Sockets;
using log4net;

namespace SharedClasses.Protocol
{
    public class ContributionNotificationSerialiser : ISerialiser
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (ContributionNotificationSerialiser));

        private readonly ContributionSerialiser serialiser = new ContributionSerialiser();

        public void Serialise(IMessage contributionNotificationMessage, NetworkStream stream)
        {
            MessageUtilities.SerialiseMessageIdentifier(contributionNotificationMessage.GetMessageIdentifier(), stream);

            var message = contributionNotificationMessage as ContributionNotification;

            if (message != null)
            {
                Log.Debug("Waiting for a contribution notification message to serialise");
                serialiser.Serialise(message.Contribution, stream);
                Log.Info("Contribution notification message serialised");
            }
            else
            {
                Log.Warn("No message to be serialised, message object is null");
            }
        }

        public IMessage Deserialise(NetworkStream stream)
        {
            Log.Debug("Waiting for a contribution notification message to deserialise");
            var notification = new ContributionNotification(serialiser.Deserialise(stream));
            Log.Info("Contribution notification message deserialised");
            return notification;
        }
    }
}