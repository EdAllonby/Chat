using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using log4net;
using SharedClasses.Domain;
using SharedClasses.Message;

namespace SharedClasses.Serialiser
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

            binaryFormatter.Serialize(stream, message);
            Log.InfoFormat("{0} serialised and sent to network stream", message);
        }

        public void Serialise(IMessage message, NetworkStream stream)
        {
            Serialise((UserNotification) message, stream);
        }

        public IMessage Deserialise(NetworkStream networkStream)
        {
            var userNotification = (UserNotification) binaryFormatter.Deserialize(networkStream);
            Log.InfoFormat("Network stream has received data and deserialised to a {0} object", userNotification.Identifier);
            return userNotification;
        }
    }
}