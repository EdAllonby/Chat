using System;
using SharedClasses.Protocol;

namespace SharedClasses
{
    /// <summary>
    /// Used as a holder of the <see cref="IMessage" /> object when an event is fired so spectators can use it
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