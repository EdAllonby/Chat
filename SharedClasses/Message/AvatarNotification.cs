using System;
using SharedClasses.Domain;

namespace SharedClasses.Message
{
    /// <summary>
    /// Packages a <see cref="Avatar" /> and a <see cref="NotificationType" /> for the Server to send to the Client.
    /// </summary>
    [Serializable]
    public sealed class AvatarNotification : IMessage
    {
        public AvatarNotification(Avatar avatar, NotificationType notificationType)
        {
            Avatar = avatar;
            NotificationType = notificationType;
        }

        public Avatar Avatar { get; private set; }

        public NotificationType NotificationType { get; private set; }

        public MessageIdentifier MessageIdentifier
        {
            get { return MessageIdentifier.AvatarNotification; }
        }
    }
}