using System;
using SharedClasses.Domain;

namespace SharedClasses.Message
{
    [Serializable]
    public sealed class UserTypingNotification : IMessage
    {
        private readonly NotificationType notificationType;
        private readonly UserTyping userTyping;

        public UserTypingNotification(UserTyping userTyping, NotificationType notificationType)
        {
            this.userTyping = userTyping;
            this.notificationType = notificationType;
        }

        public UserTyping UserTyping
        {
            get { return userTyping; }
        }

        public NotificationType NotificationType
        {
            get { return notificationType; }
        }

        public MessageIdentifier MessageIdentifier
        {
            get { return MessageIdentifier.UserTypingNotification; }
        }
    }
}