using SharedClasses.Domain;

namespace ChatClient.Models.ChatModel
{
    /// <summary>
    /// Represents the message information that a <see cref="TextContribution"/> has.
    /// This makes it easier to display useful information without polluting the domain model.
    /// </summary>
    public sealed class UserMessageModel
    {
        private readonly string message;
        private readonly string messageDetails;

        public UserMessageModel(string message, string messageDetails)
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