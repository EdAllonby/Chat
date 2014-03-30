using System.Net.Sockets;
using log4net;

namespace SharedClasses.Protocol
{
    public class ContributionNotificationSerialiser : ISerialise
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (ContributionNotificationSerialiser));

        private readonly ContributionSerialiser serialiser = new ContributionSerialiser();

        public void Serialise(IMessage notification, NetworkStream stream)
        {
            MessageType.Serialise(typeof (ContributionNotification), stream);
            var message = notification as ContributionNotification;
            Log.Debug("Waiting for a contribution notification message to serialise");

            if (message != null)
            {
                serialiser.Serialise(message.Contribution, stream);
            }

            Log.Info("Contribution notification message serialised");
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