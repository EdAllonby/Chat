﻿using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using log4net;
using SharedClasses.Message;

namespace SharedClasses.Serialiser
{
    /// <summary>
    /// Used to Serialise and Deserialise a <see cref="ParticipationSnapshotRequest" /> object.
    /// </summary>
    internal sealed class ParticipationSnapshotRequestSerialiser : ISerialiser<ParticipationSnapshotRequest>
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (ConversationSnapshotRequestSerialiser));

        private readonly BinaryFormatter binaryFormatter = new BinaryFormatter();

        private readonly MessageIdentifierSerialiser messageIdentifierSerialiser = new MessageIdentifierSerialiser();

        public void Serialise(ParticipationSnapshotRequest message, NetworkStream stream)
        {
            messageIdentifierSerialiser.SerialiseMessageIdentifier(message.Identifier, stream);

            binaryFormatter.Serialize(stream, message);
            Log.InfoFormat("{0} serialised and sent to network stream", message.Identifier);
        }

        public void Serialise(IMessage message, NetworkStream stream)
        {
            Serialise((ParticipationSnapshotRequest) message, stream);
        }

        public IMessage Deserialise(NetworkStream networkStream)
        {
            var participationSnapshotRequest = (ParticipationSnapshotRequest) binaryFormatter.Deserialize(networkStream);
            Log.InfoFormat("Network stream has received data and deserialised to a {0} object", participationSnapshotRequest.Identifier);
            return participationSnapshotRequest;
        }
    }
}