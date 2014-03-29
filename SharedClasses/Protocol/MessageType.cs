using System;
using System.Net.Sockets;
using log4net;

namespace SharedClasses.Protocol
{
    /// <summary>
    /// This class is used to hold the type of message identifier, and related utility methods.
    /// </summary>
    public class MessageType
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (MessageType));

        public int Identifier { get; private set; }

        public MessageType(int identifier)
        {
            Identifier = identifier;
        }

        public static int GetMessageType(NetworkStream stream)
        {
            var messageTypeBuffer = new byte[4];
            stream.Read(messageTypeBuffer, 0, 4);
            int messageIdentifier = BitConverter.ToInt32(messageTypeBuffer, 0);
            Log.Debug("Message Identifier " + messageIdentifier + " received from client");
            return messageIdentifier;
        }

        public static void SendMessageType(int messageIdentifier, NetworkStream stream)
        {
            stream.Write(BitConverter.GetBytes(messageIdentifier), 0, 4);
            Log.Debug("Sent Message Identifier: " + messageIdentifier + " to stream");
        }
    }
}