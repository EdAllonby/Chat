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
        private readonly int contributionId;
        private readonly int contributorUserId;
        private readonly int conversationId;
        private readonly string message;
        private readonly DateTime messageTimeStamp;

        /// <summary>
        /// Create a contribution that will later get assigned an ID.
        /// </summary>
        public Contribution(int contributorUserId, string message, int conversationId)
        {
            Contract.Requires(contributorUserId > 0);
            Contract.Requires(conversationId > 0);

            this.contributorUserId = contributorUserId;
            this.message = message;
            this.conversationId = conversationId;
        }

        /// <summary>
        /// Creates a complete contribution entity
        /// </summary>
        /// <param name="contributionId">The unique ID of the entity.</param>
        /// <param name="incompleteContribution">The extra details of the <see cref="Contribution"/>.</param>
        public Contribution(int contributionId, Contribution incompleteContribution)
            : this(incompleteContribution.ContributorUserId, incompleteContribution.Message, incompleteContribution.ConversationId)
        {
            Contract.Requires(contributionId > 0);
            Contract.Requires(incompleteContribution != null);

            this.contributionId = contributionId;
            messageTimeStamp = DateTime.Now;
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
        public int ContributionId
        {
            get { return contributionId; }
        }

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
        public string Message
        {
            get { return message; }
        }

        /// <summary>
        /// The time the server received the message.
        /// </summary>
        public DateTime MessageTimeStamp
        {
            get { return messageTimeStamp; }
        }

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