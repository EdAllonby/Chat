using System;
using SharedClasses.Domain;

namespace SharedClasses.Message
{
    /// <summary>
    /// Packages a <see cref="User" /> for the Client to send to the Server
    /// </summary>
    [Serializable]
    public sealed class LoginResponse : IMessage
    {
        public LoginResponse(User user, LoginResult loginResult)
        {
            User = user;
            LoginResult = loginResult;
        }

        public LoginResponse(LoginResult loginResult)
        {
            LoginResult = loginResult;
        }

        /// <summary>
        /// Whether or not the Client is allowed on to the Server.
        /// </summary>
        public LoginResult LoginResult { get; private set; }

        /// <summary>
        /// The <see cref="User" /> object created by the Server.
        /// </summary>
        public User User { get; private set; }

        /// <summary>
        /// The type of message this is.
        /// </summary>
        public MessageIdentifier MessageIdentifier => MessageIdentifier.LoginResponse;
    }
}