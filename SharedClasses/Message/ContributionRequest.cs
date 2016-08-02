using System;
using SharedClasses.Domain;

namespace SharedClasses.Message
{
    /// <summary>
    /// Packages a <see cref="Contribution" /> without an Id for the Client to send to the Server.
    /// </summary>
    [Serializable]
    public sealed class ContributionRequest : IMessage
    {
        public ContributionRequest(IContribution contribution)
        {
            Contribution = contribution;
        }

        public IContribution Contribution { get; private set; }

        public MessageIdentifier MessageIdentifier => MessageIdentifier.ContributionRequest;
    }
}