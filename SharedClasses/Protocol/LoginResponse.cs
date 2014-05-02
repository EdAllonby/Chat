using System;
using SharedClasses.Domain;

namespace SharedClasses.Protocol
{
    /// <summary>
    /// Packages a <see cref="User"/> for the Client to send to the Server
    /// </summary>
    [Serializable]
    public sealed class LoginResponse : IMessage
    {
        public LoginResponse(User user)
        {
            User = user;
            Identifier = MessageNumber.LoginResponse;
        }

        public User User { get; private set; }

        public int Identifier { get; private set; }
    }
}
