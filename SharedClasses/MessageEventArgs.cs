using System;
using System.Net.Sockets;
using SharedClasses.Domain;

namespace SharedClasses.Protocol
{
    /// <summary>
    ///     Used as a holder of the <see cref="IMessage" /> object when an event is fired so spectators can use it
    /// </summary>
    public class MessageEventArgs : EventArgs
    {
        public MessageEventArgs(IMessage message, NetworkStream sendersStream)
        {
            Message = message;
            SendersStream = sendersStream;
        }

        public IMessage Message { get; private set; }

        public NetworkStream SendersStream { get; private set; }
    }
}