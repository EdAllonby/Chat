using SharedClasses.Domain;

namespace ChatClient.Models
{
    /// <summary>
    /// Represents the message information that a <see cref="Conversation"/> has.
    /// This makes it easier to display useful information without polluting the domain model.
    /// </summary>
    public sealed class UserMessage
    {
        private readonly string message;
        private readonly string messageDetails;

        public UserMessage(string message, string messageDetails)
        {
            this.message = message;
            this.messageDetails = messageDetails;
        }

        public string Message
        {
            get { return message; }
        }

        public string MessageDetails
        {
            get { return messageDetails; }
        }
    }
}