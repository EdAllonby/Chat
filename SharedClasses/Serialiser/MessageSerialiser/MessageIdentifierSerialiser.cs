using System;
using System.Diagnostics.Contracts;
using System.Net.Sockets;
using log4net;

namespace SharedClasses.Serialiser.MessageSerialiser
{
    /// <summary>
    /// This static class is used define what message gets what identifier,
    /// and used to serialise and deserialise Message Identifiers to their related Typed
    /// </summary>
    public sealed class MessageIdentifierSerialiser
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (MessageIdentifierSerialiser));

        public void Serialise(NetworkStream stream, MessageIdentifier messageIdentifier)
        {
            Contract.Requires(stream != null);

            stream.Write(BitConverter.GetBytes((int) messageIdentifier), 0, 4);
            Log.DebugFormat("Sent Message Identifier: {0} to stream", messageIdentifier);
        }

        public MessageIdentifier DeserialiseMessageIdentifier(NetworkStream stream)
        {
            Contract.Requires(stream != null);

            var messageTypeBuffer = new byte[4];
            stream.Read(messageTypeBuffer, 0, 4);
            int messageIdentifierNumber = BitConverter.ToInt32(messageTypeBuffer, 0);
            var messageIdentifier = (MessageIdentifier) messageIdentifierNumber;
            Log.DebugFormat("Message Identifier {0} received from client", messageIdentifier);
            return messageIdentifier;
        }
    }
}