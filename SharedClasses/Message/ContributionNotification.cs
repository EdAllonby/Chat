using System;
using SharedClasses.Domain;

namespace SharedClasses.Message
{
    /// <summary>
    /// Packages a <see cref="Contribution"/> for the Server to send to the Client
    /// </summary>
    [Serializable]
    public sealed class ContributionNotification : IMessage
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