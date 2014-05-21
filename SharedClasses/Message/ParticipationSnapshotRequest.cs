using System;

namespace SharedClasses.Message
{
    /// <summary>
    /// Requests a list of participations related to the newly connected users
    /// </summary>
    [Serializable]
    public class ParticipationSnapshotRequest : IMessage
    {
        public MessageNumber Identifier
        {
            get { return MessageNumber.ParticipationSnapshotRequest; }
        }
    }
}