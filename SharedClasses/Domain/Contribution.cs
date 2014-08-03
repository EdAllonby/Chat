using System;
using System.Diagnostics.Contracts;

namespace SharedClasses.Domain
{
    /// <summary>
    /// A fundamental domain entity object needed to model a contribution to a conversation.
    /// </summary>
    [Serializable]
    public sealed class Contribution : IEntity, IEquatable<Contribution>
    {
        private readonly int contributorUserId;
        private readonly int conversationId;
        private readonly int id;
        private readonly string message;
        private readonly DateTime messageTimeStamp;

        /// <summary>
        /// Create a contribution that will later get assigned an Id.
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
        /// <param name="id">The unique Id of the contribution entity.</param>
        /// <param name="incompleteContribution">The extra details of the <see cref="Contribution"/>.</param>
        public Contribution(int id, Contribution incompleteContribution)
            : this(incompleteContribution.ContributorUserId, incompleteContribution.Message, incompleteContribution.ConversationId)
        {
            Contract.Requires(id > 0);
            Contract.Requires(incompleteContribution != null);

            this.id = id;
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

        /// <summary>
        /// The Unique ID of this Contribution
        /// </summary>
        public int Id
        {
            get { return id; }
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

            return Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;

            return obj is Contribution && Equals((Contribution) obj);
        }

        public override int GetHashCode()
        {
            return Id;
        }
    }
}