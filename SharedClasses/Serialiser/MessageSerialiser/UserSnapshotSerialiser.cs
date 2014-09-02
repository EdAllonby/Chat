using System.Net.Sockets;
using SharedClasses.Message;

namespace SharedClasses.Serialiser.MessageSerialiser
{
    /// <summary>
    /// Used to Serialise and Deserialise a <see cref="UserSnapshot" /> object.
    /// </summary>
    internal sealed class UserSnapshotSerialiser : Serialiser<UserSnapshot>
    {
        private readonly ISerialisationType serialiser = new BinarySerialiser();

        protected override void Serialise(NetworkStream networkStream, UserSnapshot message)
        {
            serialiser.Serialise(networkStream, message);
        }

        public override IMessage Deserialise(NetworkStream networkStream)
        {
            var userSnapshot = (UserSnapshot) serialiser.Deserialise(networkStream);
            Log.InfoFormat("Network stream has received data and deserialised to a {0} object", userSnapshot.MessageIdentifier);
            return userSnapshot;
        }
    }
}