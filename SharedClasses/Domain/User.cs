using System;

namespace SharedClasses.Domain
{
    /// <summary>
    ///     Models a user in the system.
    /// </summary>
    [Serializable]
    public sealed class User
    {
        private int id;

        public User(string username, int id)
        {
            UserName = username;
            ID = id;
        }

        public string UserName { get; private set; }

        public int ID
        {
            get { return id; }

            private set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException();
                }

                id = value;
            }
        }
    }
}