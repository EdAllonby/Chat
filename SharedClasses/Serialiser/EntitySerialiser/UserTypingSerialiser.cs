using System.Diagnostics.Contracts;
using System.Net.Sockets;
using log4net;
using SharedClasses.Domain;

namespace SharedClasses.Serialiser.EntitySerialiser
{
    /// <summary>
    /// Used to serialise and deserialise the <see cref="UserTyping" /> Domain object
    /// </summary>
    internal sealed class UserTypingSerialiser
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (UserTypingSerialiser));

        private readonly ISerialisationType serialiser = new BinarySerialiser();

        public void Serialise(NetworkStream networkStream, UserTyping userTyping)
        {
            Contract.Requires(userTyping != null);
            Contract.Requires(networkStream != null);

            serialiser.Serialise(networkStream, userTyping);
            Log.Debug("User serialised and sent to network stream");
        }

        public UserTyping Deserialise(NetworkStream networkStream)
        {
            Contract.Requires(networkStream != null);

            var userTyping = (UserTyping) serialiser.Deserialise(networkStream);
            Log.Debug("Network stream has received data and deserialised to a User object");
            return userTyping;
        }
    }
}