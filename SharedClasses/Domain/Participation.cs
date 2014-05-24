using System;

namespace SharedClasses.Domain
{
    /// <summary>
    /// The relationship between a User and a Conversation
    /// </summary>
    [Serializable]
    public class Participation : IEquatable<Participation>
    {
        private readonly int conversationId;
        private readonly int userId;

        public Participation(int userId, int conversationId)
        {
            this.userId = userId;
            this.conversationId = conversationId;
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
            if (obj.GetType() != GetType()) return false;
            return Equals((Participation) obj);
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