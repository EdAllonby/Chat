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
            Notification = notificationType;
        }

        public NotificationType Notification { get; private set; }

        public User User { get; private set; }

        public MessageNumber Identifier
        {
            get { return MessageNumber.UserNotification; }
        }
    }
}