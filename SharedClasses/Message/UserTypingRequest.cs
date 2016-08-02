using System;
using SharedClasses.Domain;

namespace SharedClasses.Message
{
    [Serializable]
    public sealed class UserTypingRequest : IMessage
    {
        public UserTypingRequest(UserTyping userTyping)
        {
            UserTyping = userTyping;
        }

        public UserTyping UserTyping { get; }

        public MessageIdentifier MessageIdentifier => MessageIdentifier.UserTypingRequest;
    }
}