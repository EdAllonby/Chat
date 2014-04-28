using System;
using SharedClasses.Domain;
using SharedClasses.Protocol;

namespace SharedClasses
{
    /// <summary>
    /// Holds an <see cref="IMessage"/> and the <see cref="ClientUser"/> who sent it
    /// </summary>
    public class MessageEventArgs : EventArgs
    {
        public MessageEventArgs(IMessage message, User clientUser)
        {
            Message = message;
            ClientUser = clientUser;
        }

        public IMessage Message { get; private set; }

        public User ClientUser { get; private set; }
    }
}