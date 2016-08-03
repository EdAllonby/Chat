using log4net;
using SharedClasses.Message;

namespace SharedClasses
{
    /// <summary>
    /// Defines an <see langword="IMessageHandler" /> that is to be used for handling incoming <see cref="IMessage" />s.
    /// </summary>
    public abstract class MessageHandler<TMessage> : IMessageHandler where TMessage : IMessage
    {
        protected static readonly ILog Log = LogManager.GetLogger(typeof(MessageHandler<TMessage>));

        protected readonly IServiceRegistry ServiceRegistry;

        protected MessageHandler(IServiceRegistry serviceRegistry)
        {
            ServiceRegistry = serviceRegistry;
        }

        /// <summary>
        /// Handles the incoming <see cref="IMessage" />.
        /// </summary>
        /// <param name="message">The message that has been received and needs to be handled.</param>
        public void HandleMessage(IMessage message)
        {
            HandleMessage((TMessage)message);
        }

        /// <summary>
        /// Handle the incoming message.
        /// </summary>
        /// <param name="message">The message to handle.</param>
        protected abstract void HandleMessage(TMessage message);
    }
}