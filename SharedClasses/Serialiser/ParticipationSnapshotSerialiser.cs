using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using SharedClasses.Message;

namespace SharedClasses.Serialiser
{
    /// <summary>
    /// Used to Serialise and Deserialise a <see cref="ParticipationSnapshot" /> object.
    /// </summary>
    internal class ParticipationSnapshotSerialiser : Serialiser<ParticipationSnapshot>
    {
        private readonly BinaryFormatter binaryFormatter = new BinaryFormatter();

        private readonly MessageIdentifierSerialiser messageIdentifierSerialiser = new MessageIdentifierSerialiser();

        protected override void Serialise(ParticipationSnapshot message, NetworkStream networkStream)
        {
            messageIdentifierSerialiser.SerialiseMessageIdentifier(message.MessageIdentifier, networkStream);

            binaryFormatter.Serialize(networkStream, message);
            Log.InfoFormat("{0} serialised and sent to network stream", message.MessageIdentifier);
        }

        public override IMessage Deserialise(NetworkStream networkStream)
        {
            var participationSnapshot = (ParticipationSnapshot) binaryFormatter.Deserialize(networkStream);
            Log.InfoFormat("Network stream has received data and deserialised to a {0} object.", participationSnapshot.MessageIdentifier);
            return participationSnapshot;
        }
    }
}