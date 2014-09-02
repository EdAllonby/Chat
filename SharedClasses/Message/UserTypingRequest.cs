using System;
using SharedClasses.Domain;

namespace SharedClasses.Message
{
    [Serializable]
    public sealed class UserTypingRequest : IMessage
    {
        private readonly UserTyping userTyping;

        public UserTypingRequest(UserTyping userTyping)
        {
            this.userTyping = userTyping;
        }

        public UserTyping UserTyping
        {
            get { return userTyping; }
        }

        public MessageIdentifier MessageIdentifier
        {
            get { return MessageIdentifier.UserTypingRequest; }
        }
    }
}