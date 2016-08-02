using System;
using System.Net.Sockets;
using log4net;

namespace SharedClasses.Serialiser
{
    /// <summary>
    /// Defines  what message gets what identifier, and used to serialise and deserialise Message Identifiers to their related
    /// Type.
    /// </summary>
    public static class MessageIdentifierSerialiser
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(MessageIdentifierSerialiser));

        public static void Serialise(NetworkStream stream, MessageIdentifier messageIdentifier)
        {
            stream.Write(BitConverter.GetBytes((int)messageIdentifier), 0, 4);
            Log.DebugFormat($"Sent Message Identifier: {messageIdentifier} to stream.");
        }

        public static MessageIdentifier DeserialiseMessageIdentifier(NetworkStream stream)
        {
            var messageTypeBuffer = new byte[4];

            stream.Read(messageTypeBuffer, 0, 4);

            int messageIdentifierNumber = BitConverter.ToInt32(messageTypeBuffer, 0);
            var messageIdentifier = (MessageIdentifier)messageIdentifierNumber;

            Log.DebugFormat($"Message Identifier {messageIdentifier} received from stream.");

            return messageIdentifier;
        }
    }
}