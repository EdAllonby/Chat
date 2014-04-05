using System;

namespace SharedClasses.Domain
{
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