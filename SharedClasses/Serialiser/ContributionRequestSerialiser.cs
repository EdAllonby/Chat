using System.Net.Sockets;
using SharedClasses.Domain;
using SharedClasses.Message;

namespace SharedClasses.Serialiser
{
    /// <summary>
    /// Used to serialise and deserialise a <see cref="ContributionRequest" /> message
    /// </summary>
    internal sealed class ContributionRequestSerialiser : Serialiser<ContributionRequest>
    {
        private static readonly ContributionSerialiser ContributionSerialiser = new ContributionSerialiser();

        private readonly MessageIdentifierSerialiser messageIdentifierSerialiser = new MessageIdentifierSerialiser();

        protected override void Serialise(ContributionRequest contributionRequest, NetworkStream stream)
        {
            messageIdentifierSerialiser.SerialiseMessageIdentifier(contributionRequest.Identifier, stream);

            ContributionSerialiser.Serialise(contributionRequest.Contribution, stream);
            Log.InfoFormat("{0} message serialised", contributionRequest.Identifier);
        }

        public override IMessage Deserialise(NetworkStream stream)
        {
            Log.Debug("Waiting for a contribution request message to deserialise");
            Contribution contribution = ContributionSerialiser.Deserialise(stream);
            var contributionRequest = new ContributionRequest(
                contribution.ConversationId,
                contribution.ContributorUserId,
                contribution.Message);

            Log.Info("Contribution request message deserialised");
            return contributionRequest;
        }
    }
}