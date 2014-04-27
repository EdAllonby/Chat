using System;
using SharedClasses.Domain;

namespace SharedClasses.Protocol
{
    /// <summary>
    /// Packages  <see cref="ConversationID"/>, <see cref="SenderID"/> and <see cref="Message"/> for the Server to send to the Client
    /// </summary>
    [Serializable]
    public class ContributionNotification : IMessage
    {
        public ContributionNotification(Contribution contribution)
        {
            Contribution = contribution;
            Identifier = MessageNumber.ContributionNotification;
        }

        public Contribution Contribution { get; private set; }

        public int Identifier { get; private set; }
    }
}