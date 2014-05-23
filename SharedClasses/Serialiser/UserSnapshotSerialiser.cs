using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using SharedClasses.Message;

namespace SharedClasses.Serialiser
{
    /// <summary>
    /// Used to Serialise and Deserialise a <see cref="UserSnapshot" /> object.
    /// </summary>
    internal sealed class UserSnapshotSerialiser : Serialiser<UserSnapshot>
    {
        private readonly BinaryFormatter binaryFormatter = new BinaryFormatter();

        private readonly MessageIdentifierSerialiser messageIdentifierSerialiser = new MessageIdentifierSerialiser();

        protected override void Serialise(UserSnapshot message, NetworkStream stream)
        {
            messageIdentifierSerialiser.SerialiseMessageIdentifier(message.Identifier, stream);

            binaryFormatter.Serialize(stream, message);
            Log.InfoFormat("{0} serialised and sent to network stream", message.Identifier);
        }

        public override IMessage Deserialise(NetworkStream stream)
        {
            var userSnapshot = (UserSnapshot) binaryFormatter.Deserialize(stream);
            Log.InfoFormat("Network stream has received data and deserialised to a {0} object", userSnapshot.Identifier);
            return userSnapshot;
        }
    }
}