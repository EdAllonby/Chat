using System.Diagnostics.Contracts;
using System.Net.Sockets;
using log4net;
using SharedClasses.Domain;

namespace SharedClasses.Serialiser.EntitySerialiser
{
    /// <summary>
    /// Used to serialise and deserialise the <see cref="User" /> Domain object
    /// </summary>
    internal sealed class UserSerialiser
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (ContributionSerialiser));

        private readonly ISerialisationType serialiser = new BinarySerialiser();

        public void Serialise(NetworkStream networkStream, User user)
        {
            Contract.Requires(user != null);
            Contract.Requires(networkStream != null);

            serialiser.Serialise(networkStream, user);
            Log.Debug("User serialised and sent to network stream");
        }

        public User Deserialise(NetworkStream networkStream)
        {
            Contract.Requires(networkStream != null);

            var user = (User) serialiser.Deserialise(networkStream);
            Log.Debug("Network stream has received data and deserialised to a User object");
            return user;
        }
    }
}