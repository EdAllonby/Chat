using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using log4net;
using SharedClasses.Domain;

namespace SharedClasses.Protocol
{
    /// <summary>
    /// Used to serialise and deserialise the <see cref="User" /> Domain object
    /// </summary>
    internal sealed class UserSerialiser
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (ContributionSerialiser));

        private readonly BinaryFormatter binaryFormatter = new BinaryFormatter();

        public void Serialise(User user, NetworkStream stream)
        {
            if (stream.CanWrite)
            {
                Log.Info("Attempt to serialise User and send to stream");
                binaryFormatter.Serialize(stream, user);
                Log.Info("User serialised and sent to network stream");
            }
        }

        public User Deserialise(NetworkStream networkStream)
        {
            var user = (User) binaryFormatter.Deserialize(networkStream);
            Log.Info("Network stream has received data and deserialised to a User object");
            return user;
        }
    }
}