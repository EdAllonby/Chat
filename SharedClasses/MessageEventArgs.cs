using System;
using SharedClasses.Protocol;

namespace SharedClasses
{
    /// <summary>
    /// Holds an <see cref="IMessage"/> and the <see cref="ConnectedClient"/> who sent it
    /// </summary>
    public class MessageEventArgs : EventArgs
    {
        public MessageEventArgs(IMessage message, ConnectedClient connectedClient)
        {
            Message = message;
            ConnectedClient = connectedClient;
        }

        public IMessage Message { get; private set; }

        public ConnectedClient ConnectedClient { get; private set; }
    }
}