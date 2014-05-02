using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using log4net;
using SharedClasses.Domain;

namespace SharedClasses.Protocol
{
    /// <summary>
    /// Used to Serialise and Deserialise a <see cref="User" /> Domain object.
    /// </summary>
    internal sealed class UserNotificationSerialiser : ISerialiser<UserNotification>
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (UserNotificationSerialiser));

        private readonly BinaryFormatter binaryFormatter = new BinaryFormatter();

        private readonly MessageIdentifierSerialiser messageIdentifierSerialiser = new MessageIdentifierSerialiser();

        public void Serialise(UserNotification message, NetworkStream stream)
        {
            messageIdentifierSerialiser.SerialiseMessageIdentifier(message.Identifier, stream);

            Log.Info("Attempt to serialise UserNotification and send to stream");
            binaryFormatter.Serialize(stream, message);
            Log.Info("UserNotification serialised and sent to network stream");
        }

        public void Serialise(IMessage message, NetworkStream stream)
        {
            Serialise((UserNotification) message, stream);
        }

        public IMessage Deserialise(NetworkStream networkStream)
        {
            var userNotification = (UserNotification) binaryFormatter.Deserialize(networkStream);
            Log.Info("Network stream has received data and deserialised to a UserNotification object");
            return userNotification;
        }
    }
}