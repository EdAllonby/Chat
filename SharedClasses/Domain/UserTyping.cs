using System;

namespace SharedClasses.Domain
{
    /// <summary>
    /// Signifies when a <see cref="User" /> has started or finished typing a contribution to a conversation.
    /// </summary>
    [Serializable]
    public sealed class UserTyping : IEquatable<UserTyping>
    {
        private readonly bool isUserTyping;
        private readonly int participationId;

        public UserTyping(bool isUserTyping, int participationId)
        {
            this.isUserTyping = isUserTyping;
            this.participationId = participationId;
        }

        public bool IsUserTyping
        {
            get { return isUserTyping; }
        }

        public int ParticipationId
        {
            get { return participationId; }
        }

        #region IEquatable<UserTyping> Implementation

        public bool Equals(UserTyping other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return isUserTyping.Equals(other.isUserTyping) && participationId == other.participationId;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is UserTyping && Equals((UserTyping) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (isUserTyping.GetHashCode() * 397) ^ participationId;
            }
        }

        public static bool operator ==(UserTyping left, UserTyping right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(UserTyping left, UserTyping right)
        {
            return !Equals(left, right);
        }

        #endregion
    }
}