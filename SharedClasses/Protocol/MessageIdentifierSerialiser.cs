using System;
using System.IO;
using System.Net.Sockets;
using log4net;

namespace SharedClasses.Protocol
{
    /// <summary>
    ///     This static class is used define what message gets what identifier,
    ///     and used to serialise and deserialise Message Identifiers to their related Typed
    /// </summary>
    public class MessageIdentifierSerialiser
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (MessageIdentifierSerialiser));

        public void SerialiseMessageIdentifier(int messageIdentifier, NetworkStream stream)
        {
            stream.Write(BitConverter.GetBytes(messageIdentifier), 0, 4);
            Log.Debug("Sent Message Identifier: " + messageIdentifier + " to stream");
        }

        public int DeserialiseMessageIdentifier(NetworkStream stream)
        {
            try
            {
                var messageTypeBuffer = new byte[4];
                stream.Read(messageTypeBuffer, 0, 4);
                int messageIdentifier = BitConverter.ToInt32(messageTypeBuffer, 0);
                Log.Debug("Message Identifier " + messageIdentifier + " received from client");
                return messageIdentifier;
            }

            catch (IOException ioException)
            {
                Log.Error("connection lost between the client and the server", ioException);
                stream.Close();
            }

            //TODO: What happens if this fails, what should I return. Need to think about properly laying out try catches
            return int.MaxValue;
        }
    }
}