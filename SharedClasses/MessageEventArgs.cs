using System;
using SharedClasses.Protocol;

namespace SharedClasses
{
    public class MessageEventArgs : EventArgs
    {
        public IMessage Message { get; set; }

        public MessageEventArgs(IMessage message)
        {
            Message = message;
        }
    }
}
