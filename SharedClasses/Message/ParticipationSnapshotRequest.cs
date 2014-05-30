using System;

namespace SharedClasses.Message
{
    /// <summary>
    /// Requests a list of participations related to the newly connected users
    /// </summary>
    [Serializable]
    public class ParticipationSnapshotRequest : IMessage
    {
        public ParticipationSnapshotRequest(int userId)
        {
            UserId = userId;
        }

        public int UserId { get; private set; }

        public MessageIdentifier MessageIdentifier
        {
            get { return MessageIdentifier.ParticipationSnapshotRequest; }
        }
    }
}