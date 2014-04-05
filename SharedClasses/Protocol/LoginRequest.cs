using System;
using SharedClasses.Domain;

namespace SharedClasses.Protocol
{
    [Serializable]
    public class LoginRequest : IMessage
    {
        public LoginRequest(string userName)
        {
            UserName = userName;
            Identifier = SerialiserRegistry.IdentifiersByMessageType[typeof (LoginRequest)];
        }

        public string UserName { get; private set; }

        public int Identifier { get; private set; }
    }
}