using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using log4net;
using SharedClasses.Domain;

namespace SharedClasses.Protocol
{
    /// <summary>
    /// Used to serialise and deserialise the <see cref="Contribution" /> Domain object
    /// Both <see cref="ContributionRequest" /> and <see cref="ContributionNotification" /> use this class
    /// to do its the main serialisation work
    /// </summary>
    internal sealed class ContributionSerialiser
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (ContributionSerialiser));

        private readonly BinaryFormatter binaryFormatter = new BinaryFormatter();

        public void Serialise(Contribution contribution, NetworkStream stream)
        {
            if (stream.CanWrite)
            {
                Log.Info("Attempt to serialise Contribution and send to stream");
                binaryFormatter.Serialize(stream, contribution);
                Log.Info("Contribution serialised and sent to network stream");
            }
        }

        public Contribution Deserialise(NetworkStream networkStream)
        {
            var contribution = (Contribution) binaryFormatter.Deserialize(networkStream);
            Log.Info("Network stream has received data and deserialised to a Contribution object");
            return contribution;
        }
    }
}