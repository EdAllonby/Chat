using System;
using SharedClasses.Domain;

namespace SharedClasses.Protocol
{
    /// <summary>
    /// Used to show that a new user has logged on
    /// </summary>
    [Serializable]
    public class UserNotification : IMessage
    {
        public UserNotification(User user, NotificationType notificationType)
        {
            User = user;
            Notification = notificationType;
            Identifier = MessageNumber.UserNotification;
        }

        public NotificationType Notification { get; set; }

        public User User { get; set; }

        public int Identifier { get; private set; }
    }
}