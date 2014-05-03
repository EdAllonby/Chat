using System;
using SharedClasses.Domain;

namespace SharedClasses.Message
{
    /// <summary>
    /// Packages a <see cref="User"/> for the Client to send to the Server
    /// </summary>
    [Serializable]
    public sealed class LoginRequest : IMessage
    {
        public LoginRequest(User user)
        {
            User = user;
            Identifier = MessageNumber.LoginRequest;
        }

        public User User { get; private set; }

        public int Identifier { get; private set; }
    }
}