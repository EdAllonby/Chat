using System;
using SharedClasses.Domain;

namespace SharedClasses.Message
{
    /// <summary>
    /// Used to show that a new user has logged on
    /// </summary>
    [Serializable]
    public sealed class UserNotification : IMessage
    {
        public UserNotification(User user, NotificationType notificationType)
        {
            User = user;
            NotificationType = notificationType;
        }

        public NotificationType NotificationType { get; private set; }

        public User User { get; private set; }

        public MessageIdentifier MessageIdentifier
        {
            get { return MessageIdentifier.UserNotification; }
        }
    }
}