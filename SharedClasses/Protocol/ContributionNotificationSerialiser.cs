using System;
using System.Net.Sockets;
using log4net;

namespace SharedClasses.Protocol
{
    public class ContributionNotificationSerialiser
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (ContributionNotificationSerialiser));

        private readonly ContributionSerialiser serialiser = new ContributionSerialiser();

        public void Serialise(ContributionNotification notification, NetworkStream stream)
        {
            MessageType.SendMessageType(ContributionNotification.MessageType.Identifier, stream);

            Log.Debug("Waiting for a contribution notification message to serialise");
            serialiser.Serialise(notification.Contribution, stream);
            Log.Info("Contribution notification message serialised");
        }

        public ContributionNotification Deserialise(NetworkStream stream)
        {
            Log.Debug("Waiting for a contribution notification message to deserialise");
            var notification = new ContributionNotification(serialiser.Deserialise(stream));
            Log.Info("Contribution notification message deserialised");
            return notification;
        }
    }
}