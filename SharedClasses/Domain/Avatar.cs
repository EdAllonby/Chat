using System;
using System.Drawing;

namespace SharedClasses.Domain
{
    /// <summary>
    /// An image linked with a <see cref="User" /> used as an avatar.
    /// </summary>
    [Serializable]
    public sealed class Avatar : IEntity, IEquatable<Avatar>
    {
        /// <summary>
        /// Create a null Avatar object.
        /// </summary>
        public Avatar()
        {
        }

        public Avatar(int userId, Image userAvatar)
        {
            UserId = userId;
            UserAvatar = userAvatar;
        }

        public Avatar(int id, Avatar incompleteAvatar)
            : this(incompleteAvatar.UserId, incompleteAvatar.UserAvatar)
        {
            Id = id;
        }

        public int UserId { get; }

        public Image UserAvatar { get; }

        public int Id { get; }

        #region IEquality implementation

        public bool Equals(Avatar other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is Avatar && Equals((Avatar) obj);
        }

        public override int GetHashCode()
        {
            return Id;
        }

        #endregion
    }
}