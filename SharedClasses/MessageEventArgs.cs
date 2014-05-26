using System;
using SharedClasses.Message;

namespace SharedClasses
{
    /// <summary>
    /// Holds an <see cref="IMessage"/> and the <see cref="ClientUser"/> who sent it
    /// </summary>
    public sealed class MessageEventArgs : EventArgs
    {
        public MessageEventArgs(IMessage message)
        {
            Message = message;
        }

        public IMessage Message { get; private set; }
    }
}