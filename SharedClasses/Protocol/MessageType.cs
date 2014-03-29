using System;
using System.Collections.Generic;
using System.Net.Sockets;
using log4net;

namespace SharedClasses.Protocol
{
    /// <summary>
    ///     This static class is used to hold the type of message identifier, and related utility methods.
    /// </summary>
    public static class MessageType
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (MessageType));

        private static Dictionary<int, Type> MessageIdentifierRegistry = new Dictionary<int, Type>
        {
            {1, typeof (ContributionRequest)},
            {2, typeof (ContributionNotification)},
            {3, typeof (LoginRequest)},
        };

        private static readonly Dictionary<Type, int> MessageTypeRegistry = new Dictionary<Type, int>
        {
            {typeof (ContributionRequest), 1},
            {typeof (ContributionNotification), 2},
            {typeof (LoginRequest), 3}
        };

        public static void Serialise(Type serialiserType, NetworkStream stream)
        {
            int messageIdentifier = GetMessageIdentity(serialiserType);

            stream.Write(BitConverter.GetBytes(messageIdentifier), 0, 4);
            Log.Debug("Sent Message Identifier: " + messageIdentifier + " to stream");
        }

        public static int Deserialise(NetworkStream stream)
        {
            var messageTypeBuffer = new byte[4];
            stream.Read(messageTypeBuffer, 0, 4);
            int messageIdentifier = BitConverter.ToInt32(messageTypeBuffer, 0);
            Log.Debug("Message Identifier " + messageIdentifier + " received from client");
            return messageIdentifier;
        }

        public static int GetMessageIdentity(Type serialiserType)
        {
            return MessageTypeRegistry[serialiserType];
        }
    }
}