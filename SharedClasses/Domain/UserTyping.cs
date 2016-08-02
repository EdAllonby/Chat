using System;

namespace SharedClasses.Domain
{
    /// <summary>
    /// Signifies when a <see cref="User" /> has started or finished typing a contribution to a conversation.
    /// </summary>
    [Serializable]
    public sealed class UserTyping : IEquatable<UserTyping>, IEntity
    {
        public UserTyping(bool isUserTyping, int participationId)
        {
            IsUserTyping = isUserTyping;
            ParticipationId = participationId;
        }

        public bool IsUserTyping { get; }

        public int ParticipationId { get; }

        #region IEquatable<UserTyping> Implementation

        public bool Equals(UserTyping other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return IsUserTyping.Equals(other.IsUserTyping) && ParticipationId == other.ParticipationId;
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
                return (IsUserTyping.GetHashCode()*397) ^ ParticipationId;
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

        public int Id { get; } = 1;
    }
}