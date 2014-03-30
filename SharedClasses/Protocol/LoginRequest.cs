using System;

namespace SharedClasses.Protocol
{
    [Serializable]
    public class LoginRequest : IMessage
    {
        public string UserName { private get; set; }

        public string GetMessage()
        {
            return UserName;
        }
    }
}