using System;

namespace SharedClasses.Message
{
    /// <summary>
    /// Requests a list of currently connected users
    /// </summary>
    [Serializable]
    public sealed class UserSnapshotRequest : IMessage
    {
        public MessageNumber Identifier
        {
            get { return MessageNumber.UserSnapshotRequest; }
        }
    }
}