using System;

namespace SharedClasses.Domain
{
    [Serializable]
    public class User
    {
        public string UserName { get; private set; }

        public User(string username)
        {
            UserName = username;
        }
    }
}