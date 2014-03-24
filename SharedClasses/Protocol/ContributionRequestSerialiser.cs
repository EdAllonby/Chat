using System.Net.Sockets;

namespace SharedClasses.Protocol
{
    public class ContributionRequestSerialiser
    {
        private readonly ContributionSerialiser serialiser = new ContributionSerialiser();

        public void Serialize(ContributionRequest request, NetworkStream stream)
        {
            serialiser.Serialise(request.Contribution, stream);
        }

        public ContributionRequest Deserialize(NetworkStream stream)
        {
            var request = new ContributionRequest {Contribution = serialiser.Deserialise(stream)};
            return request;
        }
    }
}