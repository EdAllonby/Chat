using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using SharedClasses.Message;

namespace SharedClasses.Serialiser.MessageSerialiser
{
    /// <summary>
    /// Used to Serialise and Deserialise a <see cref="UserSnapshot" /> object.
    /// </summary>
    internal sealed class UserSnapshotSerialiser : Serialiser<UserSnapshot>
    {
        private readonly BinaryFormatter binaryFormatter = new BinaryFormatter();

        private readonly MessageIdentifierSerialiser messageIdentifierSerialiser = new MessageIdentifierSerialiser();

        protected override void Serialise(UserSnapshot message, NetworkStream networkStream)
        {
            messageIdentifierSerialiser.Serialise(networkStream, message.MessageIdentifier);

            binaryFormatter.Serialize(networkStream, message);
            Log.InfoFormat("{0} serialised and sent to network stream", message.MessageIdentifier);
        }

        public override IMessage Deserialise(NetworkStream networkStream)
        {
            var userSnapshot = (UserSnapshot) binaryFormatter.Deserialize(networkStream);
            Log.InfoFormat("Network stream has received data and deserialised to a {0} object", userSnapshot.MessageIdentifier);
            return userSnapshot;
        }
    }
}