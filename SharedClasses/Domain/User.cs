using System;

namespace SharedClasses.Domain
{
    /// <summary>
    ///     Models a user in the system.
    /// </summary>
    [Serializable]
    public sealed class User : IEquatable<User>
    {
        private readonly int id;

        public User(string username, int id)
        {
            UserName = username;

            if (id < 0)
            {
                throw new ArgumentOutOfRangeException();
            }

            this.id = id;
        }

        public string UserName { get; private set; }

        public int ID
        {
            get { return id; }
        }

        #region IEquatable Implementation

        public bool Equals(User other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return id == other.id && string.Equals(UserName, other.UserName);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            return obj is User && Equals((User) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (id*397) ^ UserName.GetHashCode();
            }
        }

        #endregion
    }
}