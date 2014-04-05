using System.Net.Sockets;
using log4net;
using SharedClasses.Domain;

namespace SharedClasses.Protocol
{
    public class ContributionRequestSerialiser : ISerialiser<ContributionRequest>
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (ContributionRequestSerialiser));

        private readonly MessageIdentifierSerialiser messageIdentifierSerialiser = new MessageIdentifierSerialiser();
        private readonly ContributionSerialiser serialiser = new ContributionSerialiser();

        public void Serialise(ContributionRequest contributionRequest, NetworkStream stream)
        {
            messageIdentifierSerialiser.SerialiseMessageIdentifier(contributionRequest.Identifier, stream);
            ContributionRequest message = contributionRequest;

            Log.Debug("Waiting for contribution request message to serialise");
            serialiser.Serialise(message.Contribution, stream);
            Log.Info("Contribution request message serialised");
        }

        public void Serialise(IMessage contributionRequestMessage, NetworkStream stream)
        {
            Serialise((ContributionRequest) contributionRequestMessage, stream);
        }

        public IMessage Deserialise(NetworkStream networkStream)
        {
            Log.Debug("Waiting for a contribution request message to deserialise");
            var request = new ContributionRequest(serialiser.Deserialise(networkStream));
            Log.Info("Contribution request message deserialised");
            return request;
        }
    }
}