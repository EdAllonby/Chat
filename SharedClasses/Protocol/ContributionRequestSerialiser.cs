using System.Net.Sockets;
using log4net;

namespace SharedClasses.Protocol
{
    public class ContributionRequestSerialiser
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (ContributionRequestSerialiser));

        private readonly ContributionSerialiser serialiser = new ContributionSerialiser();

        public void Serialise(ContributionRequest request, NetworkStream stream)
        {
            Log.Debug("Waiting for contribution request message to serialise");
            serialiser.Serialise(request.Contribution, stream);
            Log.Info("Contribution request message serialised");
        }

        public ContributionRequest Deserialise(NetworkStream stream)
        {
            Log.Debug("Waiting for a contribution request message to deserialise");
            var request = new ContributionRequest {Contribution = serialiser.Deserialise(stream)};
            Log.Info("Contribution request message deserialised");
            return request;
        }
    }
}