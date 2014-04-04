using System;
using SharedClasses.Protocol;

namespace SharedClasses
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
