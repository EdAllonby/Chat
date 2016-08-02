using System;
using SharedClasses.Domain;

namespace SharedClasses.Message
{
    [Serializable]
    public sealed class UserTypingNotification : IMessage
    {
        public UserTypingNotification(UserTyping userTyping, NotificationType notificationType)
        {
            UserTyping = userTyping;
            NotificationType = notificationType;
        }

        public UserTyping UserTyping { get; }

        public NotificationType NotificationType { get; }

        public MessageIdentifier MessageIdentifier
        {
            get { return MessageIdentifier.UserTypingNotification; }
        }
    }
}