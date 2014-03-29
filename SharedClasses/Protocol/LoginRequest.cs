using System;

namespace SharedClasses.Protocol
{
    [Serializable]
    public class LoginRequest
    {
        public static MessageType MessageType = new MessageType(3);

        public string UserName { get; set; }
    }
}