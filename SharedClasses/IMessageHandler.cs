using SharedClasses.Message;

namespace SharedClasses
{
    /// <summary>
    /// Defines an object that is to be used for handling incoming <see cref="IMessage"/>s.
    /// </summary>
    public interface IMessageHandler
    {
        /// <summary>
        /// Handles the incoming <see cref="IMessage"/>.
        /// </summary>
        /// <param name="message">The message that has been received and needs to be handled.</param>
        /// <param name="context">The objects needed to handle the message correctly.</param>
        void HandleMessage(IMessage message, IMessageContext context);
    }
}