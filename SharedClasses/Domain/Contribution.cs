using System;
using System.Diagnostics.Contracts;
using log4net;

namespace SharedClasses.Domain
{
    /// <summary>
    /// A fundamental domain entity object needed to model a contribution to a conversation.
    /// </summary>
    [Serializable]
    public sealed class Contribution : IEquatable<Contribution>
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (Contribution));

        // Immutable domain entity class
        private readonly int contributorUserId;
        private readonly int conversationId;
        private readonly DateTime messageTimeStamp;

        public Contribution(int contributionId, Contribution incompleteContribution)
        {
            Contract.Requires(incompleteContribution != null);

            ContributionId = contributionId;
            contributorUserId = incompleteContribution.contributorUserId;
            CreateMessage(incompleteContribution.Message);
            messageTimeStamp = DateTime.Now;
            conversationId = incompleteContribution.conversationId;
        }

        /// <summary>
        /// Create a contribution that will later get assigned an ID
        /// </summary>
        public Contribution(int contributorUserId, string text, int conversationId)
        {
            Contract.Requires(contributorUserId > 0);
            Contract.Requires(conversationId > 0);

            this.contributorUserId = contributorUserId;
            CreateMessage(text);
            this.conversationId = conversationId;
        }

        /// <summary>
        /// The Conversation ID this Contribution belongs to.
        /// </summary>
        public int ConversationId
        {
            get { return conversationId; }
        }

        /// <summary>
        /// The Unique ID of this Contribution
        /// </summary>
        public int ContributionId { get; private set; }

        /// <summary>
        /// The User who sent this Contribution message.
        /// </summary>
        public int ContributorUserId
        {
            get { return contributorUserId; }
        }

        /// <summary>
        /// The contribution text.
        /// </summary>
        public string Message { get; private set; }

        public DateTime MessageTimeStamp
        {
            get { return messageTimeStamp; }
        }

        public bool Equals(Contribution other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return ContributionId == other.ContributionId;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is Contribution && Equals((Contribution) obj);
        }

        public override int GetHashCode()
        {
            return ContributionId;
        }

        public override string ToString()
        {
            return Message + " @ " + messageTimeStamp;
        }

        private void CreateMessage(string messageText)
        {
            SetTextOfMessage(messageText ?? String.Empty);
        }

        private void SetTextOfMessage(string messageText)
        {
            Message = messageText;
            Log.Debug("Contribution text set: " + Message);
        }
    }
}