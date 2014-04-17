using System;

namespace SharedClasses.Protocol
{
    [Serializable]
    public class LoginResponse : IMessage
    {
        public LoginResponse()
        {
            Identifier = MessageNumber.LoginResponse;
        }

        public int Identifier { get; private set; }
    }
}