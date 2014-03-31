using System.Net.Sockets;
using log4net;

namespace SharedClasses.Protocol
{
    public class ContributionRequestSerialiser : ISerialiser
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (ContributionRequestSerialiser));

        private readonly ContributionSerialiser serialiser = new ContributionSerialiser();

        public void Serialise(IMessage request, NetworkStream stream)
        {
            MessageUtilities.SerialiseMessageIdentifier(typeof (ContributionRequest), stream);
            var message = request as ContributionRequest;

            Log.Debug("Waiting for contribution request message to serialise");

            if (message != null)
            {
                serialiser.Serialise(message.Contribution, stream);
            }

            Log.Info("Contribution request message serialised");
        }

        public IMessage Deserialise(NetworkStream stream)
        {
            Log.Debug("Waiting for a contribution request message to deserialise");
            var request = new ContributionRequest(serialiser.Deserialise(stream));
            Log.Info("Contribution request message deserialised");
            return request;
        }
    }
}