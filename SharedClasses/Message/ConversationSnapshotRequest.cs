using System;

namespace SharedClasses.Message
{
    /// <summary>
    /// Requests a list of conversations related to the newly connected users
    /// </summary>
    [Serializable]
    public sealed class ConversationSnapshotRequest : IMessage
    {
        public ConversationSnapshotRequest(int userId)
        {
            UserId = userId;
        }

        public int UserId { get; private set; }

        public MessageNumber Identifier
        {
            get { return MessageNumber.ConversationSnapshotRequest; }
        }
    }
}