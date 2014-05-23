using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using SharedClasses.Message;

namespace SharedClasses.Serialiser
{
    /// <summary>
    /// Used to Serialise and Deserialise a <see cref="ParticipationSnapshotRequest" /> object.
    /// </summary>
    internal sealed class ParticipationSnapshotRequestSerialiser : Serialiser<ParticipationSnapshotRequest>
    {
        private readonly BinaryFormatter binaryFormatter = new BinaryFormatter();

        private readonly MessageIdentifierSerialiser messageIdentifierSerialiser = new MessageIdentifierSerialiser();

        protected override void Serialise(ParticipationSnapshotRequest message, NetworkStream stream)
        {
            messageIdentifierSerialiser.SerialiseMessageIdentifier(message.Identifier, stream);

            binaryFormatter.Serialize(stream, message);
            Log.InfoFormat("{0} serialised and sent to network stream", message.Identifier);
        }

        public override IMessage Deserialise(NetworkStream stream)
        {
            var participationSnapshotRequest = (ParticipationSnapshotRequest) binaryFormatter.Deserialize(stream);
            Log.InfoFormat("Network stream has received data and deserialised to a {0} object", participationSnapshotRequest.Identifier);
            return participationSnapshotRequest;
        }
    }
}