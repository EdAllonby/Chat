using System;
using SharedClasses.Domain;

namespace SharedClasses.Protocol
{
    /// <summary>
    ///     Used as a holder of the <see cref="IMessage" /> object when an event is fired so spectators can use it
    /// </summary>
    public class MessageEventArgs : EventArgs
    {
        public MessageEventArgs(IMessage message)
        {
            Message = message;
        }

        public IMessage Message { get; private set; }
    }
}