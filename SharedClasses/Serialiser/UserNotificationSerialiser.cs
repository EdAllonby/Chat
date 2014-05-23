using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using SharedClasses.Domain;
using SharedClasses.Message;

namespace SharedClasses.Serialiser
{
    /// <summary>
    /// Used to Serialise and Deserialise a <see cref="User" /> Domain object.
    /// </summary>
    internal sealed class UserNotificationSerialiser : Serialiser<UserNotification>
    {
        private readonly BinaryFormatter binaryFormatter = new BinaryFormatter();

        private readonly MessageIdentifierSerialiser messageIdentifierSerialiser = new MessageIdentifierSerialiser();

        protected override void Serialise(UserNotification message, NetworkStream stream)
        {
            messageIdentifierSerialiser.SerialiseMessageIdentifier(message.Identifier, stream);

            binaryFormatter.Serialize(stream, message);
            Log.InfoFormat("{0} serialised and sent to network stream", message);
        }

        public override IMessage Deserialise(NetworkStream stream)
        {
            var userNotification = (UserNotification) binaryFormatter.Deserialize(stream);
            Log.InfoFormat("Network stream has received data and deserialised to a {0} object", userNotification.Identifier);
            return userNotification;
        }
    }
}