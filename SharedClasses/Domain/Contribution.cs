using System;
using System.Diagnostics.Contracts;

namespace SharedClasses.Domain
{
    /// <summary>
    /// A fundamental domain entity object needed to model a contribution to a conversation.
    /// </summary>
    [Serializable]
    public sealed class Contribution : IEquatable<Contribution>
    {
        // Immutable domain entity class
        private readonly int contributorUserId;
        private readonly int conversationId;

        /// <summary>
        /// Creates a complete contribution entity
        /// </summary>
        /// <param name="contributionId">The unique ID of the entity.</param>
        /// <param name="incompleteContribution">The extra details of the <see cref="Contribution"/>.</param>
        public Contribution(int contributionId, Contribution incompleteContribution)
        {
            Contract.Requires(incompleteContribution != null);

            ContributionId = contributionId;
            contributorUserId = incompleteContribution.contributorUserId;
            Message = incompleteContribution.Message;
            MessageTimeStamp = DateTime.Now;
            conversationId = incompleteContribution.conversationId;
        }

        /// <summary>
        /// Create a contribution that will later get assigned an ID.
        /// </summary>
        public Contribution(int contributorUserId, string message, int conversationId)
        {
            Contract.Requires(contributorUserId > 0);
            Contract.Requires(conversationId > 0);

            this.contributorUserId = contributorUserId;
            Message = message;
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
        /// The contribution message.
        /// </summary>
        public string Message { get; private set; }

        public DateTime MessageTimeStamp { get; private set; }

        public bool Equals(Contribution other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }

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
    }
}