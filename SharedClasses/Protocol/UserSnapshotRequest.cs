using System;
using SharedClasses.Domain;

namespace SharedClasses.Protocol
{
    [Serializable]
    public class UserSnapshotRequest : IMessage
    {
        public UserSnapshotRequest()
        {
            Identifier = SerialiserRegistry.IdentifiersByMessageType[typeof(UserSnapshotRequest)];
        }

        public int Identifier { get; private set; }
    }
}