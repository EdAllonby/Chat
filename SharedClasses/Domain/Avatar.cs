using System;
using System.Diagnostics.Contracts;
using System.Drawing;

namespace SharedClasses.Domain
{
    /// <summary>
    /// An image linked with a <see cref="User"/> used as an avatar.
    /// </summary>
    [Serializable]
    public sealed class Avatar : IEntity, IEquatable<Avatar>
    {
        private readonly int id;
        private readonly Image userAvatar;
        private readonly int userId;

        /// <summary>
        /// Create a null Avatar object.
        /// </summary>
        public Avatar()
        {
        }

        public Avatar(int userId, Image userAvatar)
        {
            Contract.Requires(userId > 0);
            Contract.Requires(userAvatar != null);

            this.userId = userId;
            this.userAvatar = userAvatar;
        }

        public Avatar(int id, Avatar incompleteAvatar)
            : this(incompleteAvatar.UserId, incompleteAvatar.UserAvatar)
        {
            Contract.Requires(incompleteAvatar != null);
            Contract.Requires(id > 0);
            Contract.Requires(incompleteAvatar.UserId > 0);
            Contract.Requires(incompleteAvatar.UserAvatar != null);

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

        public Image UserAvatar
        {
            get { return userAvatar; }
        }

        #region IEquality implementation

        public bool Equals(Avatar other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return id == other.id;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is Avatar && Equals((Avatar) obj);
        }

        public override int GetHashCode()
        {
            return id;
        }

        #endregion
    }
}