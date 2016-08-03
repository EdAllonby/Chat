using SharedClasses.Message;

namespace SharedClasses
{
    public interface IMessageHandler
    {
        /// <summary>
        /// Handles the incoming <see cref="IMessage" />.
        /// </summary>
        /// <param name="message">The message that has been received and needs to be handled.</param>
        void HandleMessage(IMessage message);
    }
}