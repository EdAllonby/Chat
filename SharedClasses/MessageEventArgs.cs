using System;
using System.Diagnostics.Contracts;
using SharedClasses.Message;

namespace SharedClasses
{
    /// <summary>
    /// Holds an <see cref="IMessage"/>.
    /// </summary>
    public sealed class MessageEventArgs : EventArgs
    {
        public MessageEventArgs(IMessage message)
        {
            Contract.Requires(message != null);

            Message = message;
        }

        /// <summary>
        /// The message being carried.
        /// </summary>
        public IMessage Message { get; private set; }
    }
}