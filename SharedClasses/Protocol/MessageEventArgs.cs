using System;

namespace SharedClasses.Protocol
{
    public class MessageEventArgs : EventArgs
    {
        public IMessage Message { get; private set; }

        public MessageEventArgs(IMessage message)
        {
            Message = message;
        }
    }
}
