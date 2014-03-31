using System;

namespace SharedClasses.Protocol
{
    [Serializable]
    public class LoginRequest : IMessage
    {
        public LoginRequest(string userName)
        {
            UserName = userName;
        }
        public string UserName { get; private set; }

        public string GetMessage()
        {
            return UserName;
        }

        public int GetMessageIdentifier()
        {
            return MessageUtilities.GetMessageIdentifier(typeof(LoginRequest));
        }
    }
}