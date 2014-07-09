﻿using System;
using System.Diagnostics.Contracts;
using SharedClasses.Domain;

namespace SharedClasses.Message
{
    /// <summary>
    /// Packages a <see cref="Contribution"/> for the Server to send to the Client
    /// </summary>
    [Serializable]
    public sealed class ContributionNotification : IMessage
    {
        public ContributionNotification(Contribution contribution, NotificationType notificationType)
        {
            Contract.Requires(contribution != null);
            Contract.Requires(contribution.ContributionId > 0);

            Contribution = contribution;
            NotificationType = notificationType;
        }

        public Contribution Contribution { get; private set; }

        public NotificationType NotificationType { get; private set; }

        public MessageIdentifier MessageIdentifier
        {
            get { return MessageIdentifier.ContributionNotification; }
        }
    }
}