using System;

namespace SharedClasses.Protocol
{
    /// <summary>
    /// Requests a list of currently connected users
    /// </summary>
    [Serializable]
    public sealed class UserSnapshotRequest : IMessage
    {
        public UserSnapshotRequest()
        {
            Identifier = MessageNumber.UserSnapshotRequest;
        }

        public int Identifier { get; private set; }
    }
}