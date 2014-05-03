using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using log4net;
using SharedClasses.Message;

namespace SharedClasses.Serialiser
{
    /// <summary>
    /// Used to Serialise and Deserialise a <see cref="UserSnapshot" /> object.
    /// </summary>
    internal sealed class UserSnapshotSerialiser : ISerialiser<UserSnapshot>
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (UserSnapshotSerialiser));

        private readonly BinaryFormatter binaryFormatter = new BinaryFormatter();

        private readonly MessageIdentifierSerialiser messageIdentifierSerialiser = new MessageIdentifierSerialiser();

        public void Serialise(UserSnapshot message, NetworkStream stream)
        {
            messageIdentifierSerialiser.SerialiseMessageIdentifier(message.Identifier, stream);

            binaryFormatter.Serialize(stream, message);
            Log.Info("UserSnapshot serialised and sent to network stream");
        }

        public void Serialise(IMessage message, NetworkStream stream)
        {
            Serialise((UserSnapshot) message, stream);
        }

        public IMessage Deserialise(NetworkStream networkStream)
        {
            var userSnapshot = (UserSnapshot) binaryFormatter.Deserialize(networkStream);
            Log.Info("Network stream has received data and deserialised to a UserSnapshot object");
            return userSnapshot;
        }
    }
}