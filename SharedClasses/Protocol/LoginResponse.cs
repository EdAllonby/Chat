using System;
using SharedClasses.Domain;

namespace SharedClasses.Protocol
{
    [Serializable]
    public class LoginResponse : IMessage
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