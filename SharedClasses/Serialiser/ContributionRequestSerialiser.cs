using System.Net.Sockets;
using log4net;
using SharedClasses.Domain;
using SharedClasses.Message;

namespace SharedClasses.Serialiser
{
    /// <summary>
    /// Used to serialise and deserialise a <see cref="ContributionRequest" /> message
    /// </summary>
    internal sealed class ContributionRequestSerialiser : ISerialiser<ContributionRequest>
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (ContributionRequestSerialiser));

        private static readonly ContributionSerialiser ContributionSerialiser = new ContributionSerialiser();

        private readonly MessageIdentifierSerialiser messageIdentifierSerialiser = new MessageIdentifierSerialiser();

        public void Serialise(ContributionRequest contributionRequest, NetworkStream stream)
        {
            messageIdentifierSerialiser.SerialiseMessageIdentifier(contributionRequest.Identifier, stream);

            ContributionSerialiser.Serialise(contributionRequest.Contribution, stream);
            Log.InfoFormat("{0} message serialised", contributionRequest.Identifier);
        }

        public void Serialise(IMessage contributionRequestMessage, NetworkStream stream)
        {
            Serialise((ContributionRequest) contributionRequestMessage, stream);
        }

        public IMessage Deserialise(NetworkStream networkStream)
        {
            Log.Debug("Waiting for a contribution request message to deserialise");
            Contribution contribution = ContributionSerialiser.Deserialise(networkStream);
            var contributionRequest = new ContributionRequest(
                contribution.ConversationId,
                contribution.ContributorUserId,
                contribution.Message);

            Log.Info("Contribution request message deserialised");
            return contributionRequest;
        }
    }
}