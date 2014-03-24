using System.Net.Sockets;

namespace SharedClasses.Protocol
{
    public class ContributionNotificationSerialiser
    {
        private readonly ContributionSerialiser serialiser = new ContributionSerialiser();

        public void Serialize(ContributionNotification notification, NetworkStream stream)
        {
            serialiser.Serialise(notification.Contribution, stream);
        }

        public ContributionNotification Deserialize(NetworkStream stream)
        {
            var notification = new ContributionNotification {Contribution = serialiser.Deserialise(stream)};
            return notification;
        }
    }
}