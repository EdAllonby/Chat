using System;
using SharedClasses.Domain;

namespace SharedClasses.Protocol
{
    [Serializable]
    public class UserSnapshotRequest : IMessage
    {
        public UserSnapshotRequest()
        {
            Identifier = MessageNumber.UserSnapshotRequest;
        }

        public int Identifier { get; private set; }
    }
}