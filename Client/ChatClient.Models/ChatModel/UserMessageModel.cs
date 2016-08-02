using SharedClasses.Domain;

namespace ChatClient.Models.ChatModel
{
    /// <summary>
    /// Represents the message information that a <see cref="TextContribution" /> has.
    /// This makes it easier to display useful information without polluting the domain model.
    /// </summary>
    public sealed class UserMessageModel
    {
        public UserMessageModel(string message, string messageDetails)
        {
            Message = message;
            MessageDetails = messageDetails;
        }

        public string Message { get; }

        public string MessageDetails { get; }
    }
}