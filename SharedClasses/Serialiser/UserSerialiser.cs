using System.Diagnostics.Contracts;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using log4net;
using SharedClasses.Domain;

namespace SharedClasses.Serialiser
{
    /// <summary>
    /// Used to serialise and deserialise the <see cref="User" /> Domain object
    /// </summary>
    internal sealed class UserSerialiser
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (ContributionSerialiser));

        private readonly BinaryFormatter binaryFormatter = new BinaryFormatter();

        public void Serialise(User user, NetworkStream networkStream)
        {
            Contract.Requires(user != null);
            Contract.Requires(networkStream != null);

            binaryFormatter.Serialize(networkStream, user);
            Log.Debug("User serialised and sent to network stream");
        }

        public User Deserialise(NetworkStream networkStream)
        {
            Contract.Requires(networkStream != null);

            var user = (User) binaryFormatter.Deserialize(networkStream);
            Log.Debug("Network stream has received data and deserialised to a User object");
            return user;
        }
    }
}