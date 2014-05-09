using System;
using System.Diagnostics.Contracts;
using SharedClasses.Domain;

namespace SharedClasses.Message
{
    /// <summary>
    /// Packages a <see cref="Contribution"/> without an Id for the Client to send to the Server
    /// </summary>
    [Serializable]
    public sealed class ContributionRequest : IMessage
    {
        public ContributionRequest(int conversationID, int senderID, string message)
        {
            Contract.Requires(senderID > 0);
            Contract.Requires(conversationID > 0);

            Contribution = new Contribution(senderID, message, conversationID);
            Identifier = MessageNumber.ContributionRequest;
        }

        public Contribution Contribution { get; private set; }

        public int Identifier { get; private set; }
    }
}