using System;
using SharedClasses.Domain;

namespace SharedClasses.Protocol
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
            Identifier = MessageNumber.UserNotification;
        }

        public NotificationType Notification { get; private set; }

        public User User { get; private set; }

        public int Identifier { get; private set; }
    }
}