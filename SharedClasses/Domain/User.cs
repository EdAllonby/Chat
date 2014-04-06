using System;

namespace SharedClasses.Domain
{
    /// <summary>
    ///     Models a user in the system.
    /// </summary>
    [Serializable]
    public class User
    {
        public User(string username)
        {
            UserName = username;
        }

        public string UserName { get; private set; }
    }
}