using System;

namespace SharedClasses.Protocol
{
    [Serializable]
    public class LoginRequest
    {
        public string UserName { get; set; }
        public int MessageType = 3;
    }
}