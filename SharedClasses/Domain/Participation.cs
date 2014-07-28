using System;
using System.Diagnostics.Contracts;

namespace SharedClasses.Domain
{
    /// <summary>
    /// The relationship between a User and a Conversation
    /// </summary>
    [Serializable]
    public class Participation : IEntity, IEquatable<Participation>
    {
        private readonly int conversationId;
        private readonly int id;
        private readonly int userId;

        /// <summary>
        /// Create an incomplete Participation entity without an Id.
        /// </summary>
        /// <param name="userId">The identity of the user to link to a conversation.</param>
        /// <param name="conversationId">The identity of the conversation that the user wants to link to.</param>
        public Participation(int userId, int conversationId)
        {
            Contract.Requires(userId > 0);
            Contract.Requires(conversationId > 0);

            this.userId = userId;
            this.conversationId = conversationId;
        }

        /// <summary>
        /// Creates a new participation entity.
        /// </summary>
        /// <param name="id">The identity of this participation entity object.</param>
        /// <param name="userId">The identity of the user to link to a conversation.</param>
        /// <param name="conversationId">The identity of the conversation that the user wants to link to.</param>
        public Participation(int id, int userId, int conversationId)
            : this(userId, conversationId)
        {
            Contract.Requires(conversationId > 0);
            Contract.Requires(userId > 0);
            Contract.Requires(id > 0);

            this.id = id;
        }

        public int Id
        {
            get { return id; }
        }

        public int UserId
        {
            get { return userId; }
        }

        public int ConversationId
        {
            get { return conversationId; }
        }

        public bool Equals(Participation other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            return conversationId == other.conversationId && userId == other.userId;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;

            return obj.GetType() == GetType() && Equals((Participation) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (conversationId*397) ^ userId;
            }
        }
    }
}