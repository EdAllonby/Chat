using SharedClasses.Message;

namespace ChatClient.Services
{
    /// <summary>
    /// Handles <see cref="IMessage"/>s that a client receives.
    /// </summary>
    internal interface IClientMessageHandler
    {
        /// <summary>
        /// Handles the incoming <see cref="IMessage"/>.
        /// </summary>
        /// <param name="message">The message that has been received and needs to be handled.</param>
        /// <param name="context">The objects needed to handle the message correctly.</param>
        void HandleMessage(IMessage message, IClientMessageContext context);
    }
}