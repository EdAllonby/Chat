using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using log4net;

namespace SharedClasses.Protocol
{
    /// <summary>
    /// Used to serialise and deserialise a <see cref="ContributionNotification" /> object
    /// </summary>
    public class ContributionNotificationSerialiser : ISerialiser<ContributionNotification>
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (ContributionNotificationSerialiser));

        private readonly MessageIdentifierSerialiser messageIdentifierSerialiser = new MessageIdentifierSerialiser();
        private readonly BinaryFormatter serialiser = new BinaryFormatter();

        #region Serialise

        public void Serialise(ContributionNotification contributionNotificationMessage, NetworkStream stream)
        {
            messageIdentifierSerialiser.SerialiseMessageIdentifier(MessageNumber.ContributionNotification, stream);

            Log.Debug("Waiting for a contribution notification message to serialise");
            serialiser.Serialize(stream, contributionNotificationMessage);
            Log.Info("Contribution notification message serialised");
        }

        public void Serialise(IMessage contributionNotificationMessage, NetworkStream stream)
        {
            Serialise((ContributionNotification) contributionNotificationMessage, stream);
        }

        #endregion

        #region Deserialise

        public IMessage Deserialise(NetworkStream networkStream)
        {
            Log.Debug("Waiting for a contribution notification message to deserialise");
            var notification = (ContributionNotification) serialiser.Deserialize(networkStream);
            Log.Info("Contribution notification message deserialised");
            return notification;
        }

        #endregion
    }
}