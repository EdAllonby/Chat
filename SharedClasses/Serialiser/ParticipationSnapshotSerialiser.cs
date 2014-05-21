using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using log4net;
using SharedClasses.Message;

namespace SharedClasses.Serialiser
{
    /// <summary>
    /// Used to Serialise and Deserialise a <see cref="ParticipationSnapshot" /> object.
    /// </summary>
    internal class ParticipationSnapshotSerialiser : ISerialiser<ParticipationSnapshot>
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (ConversationSnapshotSerialiser));

        private readonly BinaryFormatter binaryFormatter = new BinaryFormatter();

        private readonly MessageIdentifierSerialiser messageIdentifierSerialiser = new MessageIdentifierSerialiser();

        public void Serialise(ParticipationSnapshot message, NetworkStream stream)
        {
            messageIdentifierSerialiser.SerialiseMessageIdentifier(message.Identifier, stream);

            binaryFormatter.Serialize(stream, message);
            Log.InfoFormat("{0} serialised and sent to network stream", message.Identifier);
        }

        public void Serialise(IMessage message, NetworkStream stream)
        {
            Serialise((ParticipationSnapshot) message, stream);
        }

        public IMessage Deserialise(NetworkStream networkStream)
        {
            var participationSnapshot = (ParticipationSnapshot) binaryFormatter.Deserialize(networkStream);
            Log.InfoFormat("Network stream has received data and deserialised to a {0} object.", participationSnapshot.Identifier);
            return participationSnapshot;
        }
    }
}