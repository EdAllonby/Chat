using System.Net.Sockets;
using log4net;
using SharedClasses.Domain;
using SharedClasses.Message;

namespace SharedClasses.Serialiser.EntitySerialiser
{
    /// <summary>
    /// Used to serialise and deserialise the <see cref="Avatar" /> Domain object.
    /// Both <see cref="AvatarRequest" /> and <see cref="AvatarNotification" /> use this class to do its the main serialisation
    /// work.
    /// </summary>
    internal sealed class AvatarSerialiser
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(AvatarSerialiser));

        private readonly ISerialisationType serialiser = new BinarySerialiser();

        public void Serialise(NetworkStream networkStream, Avatar avatar)
        {
            serialiser.Serialise(networkStream, avatar);
        }

        public Avatar Deserialise(NetworkStream networkStream)
        {
            var avatar = (Avatar) serialiser.Deserialise(networkStream);
            Log.Debug("Network stream has received data and deserialised to an Avatar object.");
            return avatar;
        }
    }
}