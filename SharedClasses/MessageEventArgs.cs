using System;
using SharedClasses.Message;

namespace SharedClasses
{
    /// <summary>
    /// Holds an <see cref="IMessage"/> and the <see cref="ClientUser"/> who sent it
    /// </summary>
    public sealed class MessageEventArgs : EventArgs
    {
        public MessageEventArgs(IMessage message, int clientUserId)
        {
            Message = message;
            ClientUserId = clientUserId;
        }

        public IMessage Message { get; private set; }

        public int ClientUserId { get; private set; }
    }
}