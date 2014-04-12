using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using log4net;

namespace SharedClasses.Protocol
{
    internal class UserSnapshotRequestSerialiser : ISerialiser<UserSnapshotRequest>
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (UserSnapshotRequest));

        private readonly BinaryFormatter binaryFormatter = new BinaryFormatter();

        private readonly MessageIdentifierSerialiser messageIdentifierSerialiser = new MessageIdentifierSerialiser();

        public void Serialise(UserSnapshotRequest message, NetworkStream stream)
        {
            messageIdentifierSerialiser.SerialiseMessageIdentifier(message.Identifier, stream);

            Log.Info("Attempt to serialise UserSnapshotRequest and send to stream");
            binaryFormatter.Serialize(stream, message);
            Log.Info("UserSnapshotRequest serialised and sent to network stream");
        }

        public void Serialise(IMessage message, NetworkStream stream)
        {
            Serialise((UserSnapshotRequest) message, stream);
        }

        public IMessage Deserialise(NetworkStream networkStream)
        {
            if (!networkStream.CanRead)
            {
                //TODO: Don't return null
                return null;
            }
            var userSnapshotRequest = (UserSnapshotRequest) binaryFormatter.Deserialize(networkStream);
            Log.Info("Network stream has received data and deserialised to a UserSnapshotRequest object");
            return userSnapshotRequest;
        }
    }
}