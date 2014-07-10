using System.Net.Sockets;
using SharedClasses.Domain;
using SharedClasses.Message;
using SharedClasses.Serialiser.EntitySerialiser;

namespace SharedClasses.Serialiser.MessageSerialiser
{
    /// <summary>
    /// Used to serialise and deserialise a <see cref="ContributionRequest" /> message
    /// </summary>
    internal sealed class ContributionRequestSerialiser : Serialiser<ContributionRequest>
    {
        private readonly ContributionSerialiser contributionSerialiser = new ContributionSerialiser();

        protected override void Serialise(NetworkStream networkStream, ContributionRequest contributionRequest)
        {
            contributionSerialiser.Serialise(networkStream, contributionRequest.Contribution);
            Log.InfoFormat("{0} message serialised", contributionRequest.MessageIdentifier);
        }

        public override IMessage Deserialise(NetworkStream networkStream)
        {
            Log.Debug("Waiting for a contribution request message to deserialise");
            Contribution contribution = contributionSerialiser.Deserialise(networkStream);
            var contributionRequest = new ContributionRequest(
                contribution.ConversationId,
                contribution.ContributorUserId,
                contribution.Message);

            Log.Info("Contribution request message deserialised");

            return contributionRequest;
        }
    }
}