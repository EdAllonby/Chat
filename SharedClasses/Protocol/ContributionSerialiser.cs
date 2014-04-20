using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using log4net;
using SharedClasses.Domain;

namespace SharedClasses.Protocol
{
    /// <summary>
    /// Used to serialise and deserialise the <see cref="Contribution" /> Domain object
    /// Both <see cref="ContributionRequest" /> and <see cref="ContributionNotification" /> use this class to do the main
    /// serialisation work
    /// </summary>
    internal class ContributionSerialiser
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (ContributionSerialiser));

        private readonly BinaryFormatter binaryFormatter = new BinaryFormatter();

        #region Serialise

        public void Serialise(Contribution clientContribution, NetworkStream stream)
        {
            if (stream.CanWrite)
            {
                Log.Info("Attempt to serialise Contribution and send to stream");
                binaryFormatter.Serialize(stream, clientContribution);
                Log.Info("Contribution serialised and sent to network stream");
            }
        }

        #endregion

        #region Deserialise

        public Contribution Deserialise(NetworkStream networkStream)
        {
            var message = (Contribution) binaryFormatter.Deserialize(networkStream);
            Log.Info("Network stream has received data and deserialised to a Contribution object");
            return message;
        }

        #endregion
    }
}