using System.Diagnostics.Contracts;
using System.Net.Sockets;
using log4net;
using SharedClasses.Domain;
using SharedClasses.Message;

namespace SharedClasses.Serialiser.EntitySerialiser
{
    /// <summary>
    /// Used to serialise and deserialise the <see cref="Contribution" /> Domain object.
    /// Both <see cref="ContributionRequest" /> and <see cref="ContributionNotification" /> use this class.
    /// to do its the main serialisation work
    /// </summary>
    internal sealed class ContributionSerialiser
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (ContributionSerialiser));

        private readonly ISerialisationType serialiser = new BinarySerialiser();

        public void Serialise(NetworkStream networkStream, Contribution contribution)
        {
            Contract.Requires(contribution != null);
            Contract.Requires(networkStream != null);

            serialiser.Serialise(networkStream, contribution);
            Log.Debug("Contribution serialised and sent to network stream");
        }

        public Contribution Deserialise(NetworkStream networkStream)
        {
            Contract.Requires(networkStream != null);

            var contribution = (Contribution) serialiser.Deserialise(networkStream);
            Log.Debug("Network stream has received data and deserialised to a Contribution object");
            return contribution;
        }
    }
}