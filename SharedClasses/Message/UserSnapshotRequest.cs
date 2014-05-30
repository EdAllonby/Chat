using System;

namespace SharedClasses.Message
{
    /// <summary>
    /// Requests a list of currently connected users
    /// </summary>
    [Serializable]
    public sealed class UserSnapshotRequest : IMessage
    {
        public UserSnapshotRequest(int userId)
        {
            UserId = userId;
        }

        public int UserId { get; private set; }

        public MessageIdentifier MessageIdentifier
        {
            get { return MessageIdentifier.UserSnapshotRequest; }
        }
    }
}