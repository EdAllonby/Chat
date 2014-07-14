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
        public ContributionRequest(int conversationId, int senderId, string message)
        {
            Contract.Requires(senderId > 0);
            Contract.Requires(conversationId > 0);

            Contribution = new Contribution(senderId, message, conversationId);
        }

        public Contribution Contribution { get; private set; }

        public MessageIdentifier MessageIdentifier
        {
            get { return MessageIdentifier.ContributionRequest; }
        }
    }
}