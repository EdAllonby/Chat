using System;
using System.Diagnostics.Contracts;
using SharedClasses.Domain;

namespace SharedClasses.Message
{
    /// <summary>
    /// Packages a <see cref="Contribution"/> without an Id for the Client to send to the Server.
    /// </summary>
    [Serializable]
    public sealed class ContributionRequest : IMessage
    {
        public ContributionRequest(IContribution contribution)
        {
            Contract.Requires(contribution.ContributorUserId > 0);
            Contract.Requires(contribution.ConversationId > 0);

            Contribution = contribution;
        }

        public IContribution Contribution { get; private set; }

        public MessageIdentifier MessageIdentifier
        {
            get { return MessageIdentifier.ContributionRequest; }
        }
    }
}