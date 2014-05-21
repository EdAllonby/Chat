using System;

namespace SharedClasses.Message
{
    /// <summary>
    /// Requests a list of conversations related to the newly connected users
    /// </summary>
    [Serializable]
    public sealed class ConversationSnapshotRequest : IMessage
    {
        public MessageNumber Identifier
        {
            get { return MessageNumber.ConversationSnapshotRequest; }
        }
    }
}