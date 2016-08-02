using System;
using SharedClasses.Domain;

namespace SharedClasses.Message
{
    /// <summary>
    /// Packages a <see cref="User" /> for the Client to send to the Server
    /// </summary>
    [Serializable]
    public sealed class LoginRequest : IMessage
    {
        public LoginRequest(string username)
        {
            User = new User(username);
        }

        public User User { get; private set; }

        public MessageIdentifier MessageIdentifier => MessageIdentifier.LoginRequest;
    }
}