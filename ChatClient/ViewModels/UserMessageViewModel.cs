using SharedClasses.Domain;

namespace ChatClient.ViewModels
{
    /// <summary>
    /// Represents the message information that a <see cref="Contribution"/> has.
    /// This makes it easier to display useful information without polluting the domain model.
    /// </summary>
    public sealed class UserMessageViewModel
    {
        private readonly string message;
        private readonly string messageDetails;

        public UserMessageViewModel(string message, string messageDetails)
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