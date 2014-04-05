using System;
using SharedClasses.Domain;

namespace SharedClasses.Protocol
{
    [Serializable]
    public class UserNotification : IMessage
    {
        public NotificationType Notification { get; set; }

        public User User { get; set; }

        public int Identifier { get; private set; }

        public UserNotification(User user, NotificationType notificationType)
        {
            User = user;
            Notification = notificationType;
            Identifier = SerialiserRegistry.IdentifiersByMessageType[typeof (UserNotification)];
        }
    }
}