using System.Net.Sockets;
using SharedClasses.Message;

namespace SharedClasses.Serialiser.MessageSerialiser
{
    /// <summary>
    /// Used to Serialise and Deserialise a <see cref="ParticipationSnapshot" /> object.
    /// </summary>
    internal sealed class ParticipationSnapshotSerialiser : Serialiser<ParticipationSnapshot>
    {
        private readonly ISerialisationType serialiser = new BinarySerialiser();

        protected override void Serialise(NetworkStream networkStream, ParticipationSnapshot message)
        {
            serialiser.Serialise(networkStream, message);
            Log.InfoFormat("{0} serialised and sent to network stream", message.MessageIdentifier);
        }

        public override IMessage Deserialise(NetworkStream networkStream)
        {
            var participationSnapshot = (ParticipationSnapshot) serialiser.Deserialise(networkStream);
            Log.InfoFormat("Network stream has received data and deserialised to a {0} object.", participationSnapshot.MessageIdentifier);
            return participationSnapshot;
        }
    }
}