using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using log4net;

namespace SharedClasses.Protocol
{
    internal class UserSnapshotSerialiser : ISerialiser<UserSnapshot>
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (UserSnapshotSerialiser));

        private readonly BinaryFormatter binaryFormatter = new BinaryFormatter();

        private readonly MessageIdentifierSerialiser messageIdentifierSerialiser = new MessageIdentifierSerialiser();

        #region Serialise

        public void Serialise(UserSnapshot message, NetworkStream stream)
        {
            messageIdentifierSerialiser.SerialiseMessageIdentifier(message.Identifier, stream);

            Log.Info("Attempt to serialise UserSnapshot and send to stream");
            binaryFormatter.Serialize(stream, message);
            Log.Info("UserSnapshot serialised and sent to network stream");
        }

        public void Serialise(IMessage message, NetworkStream stream)
        {
            Serialise((UserSnapshot) message, stream);
        }

        #endregion

        #region Deserialise

        public IMessage Deserialise(NetworkStream networkStream)
        {
            if (!networkStream.CanRead)
            {
                //TODO: Don't return null
                return null;
            }

            var userSnapshot = (UserSnapshot) binaryFormatter.Deserialize(networkStream);
            Log.Info("Network stream has received data and deserialised to a UserSnapshot object");
            return userSnapshot;
        }

        #endregion
    }
}