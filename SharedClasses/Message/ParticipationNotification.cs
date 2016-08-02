using System;
using SharedClasses.Domain;

namespace SharedClasses.Message
{
    /// <summary>
    /// Packages <see cref="Participation" />s related by conversation Id for the Server to send to the Client
    /// </summary>
    [Serializable]
    public sealed class ParticipationNotification : IMessage
    {
        public ParticipationNotification(Participation participation, NotificationType notificationType)
        {
            Participation = participation;
            NotificationType = notificationType;
        }

        public Participation Participation { get; private set; }

        public NotificationType NotificationType { get; private set; }

        public MessageIdentifier MessageIdentifier
        {
            get { return MessageIdentifier.ParticipationNotification; }
        }
    }
}