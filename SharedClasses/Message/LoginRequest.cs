using System;
using System.Diagnostics.Contracts;
using SharedClasses.Domain;

namespace SharedClasses.Message
{
    /// <summary>
    /// Packages a <see cref="User"/> for the Client to send to the Server
    /// </summary>
    [Serializable]
    public sealed class LoginRequest : IMessage
    {
        public LoginRequest(string username)
        {
            Contract.Requires(username != null);

            User = new User(username);
        }

        public User User { get; private set; }

        public MessageIdentifier MessageIdentifier
        {
            get { return MessageIdentifier.LoginRequest; }
        }
    }
}