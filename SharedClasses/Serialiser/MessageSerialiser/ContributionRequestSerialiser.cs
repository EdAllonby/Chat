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
        private static readonly ContributionSerialiser ContributionSerialiser = new ContributionSerialiser();

        private readonly MessageIdentifierSerialiser messageIdentifierSerialiser = new MessageIdentifierSerialiser();

        protected override void Serialise(ContributionRequest contributionRequest, NetworkStream networkStream)
        {
            messageIdentifierSerialiser.Serialise(networkStream, contributionRequest.MessageIdentifier);

            ContributionSerialiser.Serialise(networkStream, contributionRequest.Contribution);
            Log.InfoFormat("{0} message serialised", contributionRequest.MessageIdentifier);
        }

        public override IMessage Deserialise(NetworkStream networkStream)
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