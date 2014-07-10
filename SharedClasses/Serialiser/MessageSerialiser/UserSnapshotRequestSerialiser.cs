﻿using System.Net.Sockets;
using SharedClasses.Message;

namespace SharedClasses.Serialiser.MessageSerialiser
{
    /// <summary>
    /// Used to Serialise and Deserialise a <see cref="UserSnapshotRequest" /> object.
    /// </summary>
    internal sealed class UserSnapshotRequestSerialiser : Serialiser<UserSnapshotRequest>
    {
        private readonly ISerialisationType serialiser = new BinarySerialiser();

        protected override void Serialise(NetworkStream networkStream, UserSnapshotRequest message)
        {
            serialiser.Serialise(networkStream, message);
            Log.InfoFormat("{0} serialised and sent to network stream", message.MessageIdentifier);
        }

        public override IMessage Deserialise(NetworkStream networkStream)
        {
            var userSnapshotRequest = (UserSnapshotRequest) serialiser.Deserialise(networkStream);
            Log.InfoFormat("Network stream has received data and deserialised to a {0} object", userSnapshotRequest.MessageIdentifier);
            return userSnapshotRequest;
        }
    }
}