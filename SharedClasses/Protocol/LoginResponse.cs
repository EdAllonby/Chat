using System;

namespace SharedClasses.Protocol
{
    [Serializable]
    public class LoginResponse : IMessage
    {
        public LoginResponse(int userID)
        {
            UserID = userID;
            Identifier = MessageNumber.LoginResponse;
        }

        public int UserID { get; private set; }

        public int Identifier { get; private set; }
    }
}