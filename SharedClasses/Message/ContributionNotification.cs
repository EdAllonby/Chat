using System;
using System.Diagnostics.Contracts;
using SharedClasses.Domain;

namespace SharedClasses.Message
{
    /// <summary>
    /// Packages a <see cref="Contribution"/> and a <see cref="NotificationType"/> for the Server to send to the Client.
    /// </summary>
    [Serializable]
    public sealed class ContributionNotification : IMessage
    {
        public ContributionNotification(IContribution contribution, NotificationType notificationType)
        {
            Contract.Requires(contribution != null);
            Contract.Requires(contribution.Id > 0);

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