using System;
using SharedClasses.Domain;

namespace SharedClasses.Message
{
    /// <summary>
    /// Packages a <see cref="Contribution" /> and a <see cref="NotificationType" /> for the Server to send to the Client.
    /// </summary>
    [Serializable]
    public sealed class ContributionNotification : IMessage
    {
        public ContributionNotification(IContribution contribution, NotificationType notificationType)
        {
            Contribution = contribution;
            NotificationType = notificationType;
        }

        public IContribution Contribution { get; private set; }

        public NotificationType NotificationType { get; private set; }

        public MessageIdentifier MessageIdentifier
        {
            get { return MessageIdentifier.ContributionNotification; }
        }
    }
}