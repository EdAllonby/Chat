using System;
using System.Net.Sockets;
using log4net;
using SharedClasses.Serialisation;

namespace SharedClasses
{
    [Serializable]
    public class Message
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (Message));

        private static ITcpSendBehaviour serialiseMessage = new BinaryFormat();

        private string text;
        private DateTime messageTimeStamp;

        public Message(string text)
        {
            CreateMessage(text);
            Log.Debug("Message created");
            SetSerialiseMethod(new BinaryFormat());
        }

        public static void SetSerialiseMethod(ITcpSendBehaviour method)
        {
            serialiseMessage = method;
            Log.Debug("Serialise method set to: " + serialiseMessage.GetType());
        }

        public string GetMessage()
        {
            return text + " sent at " + messageTimeStamp;
        }

        private void CreateMessage(string messageText)
        {
            SetTextOfMessage(messageText ?? String.Empty);
            SetTimeStampOfMessage(); 
        }

        public void Serialise(NetworkStream networkStream)
        {
            serialiseMessage.Serialise(networkStream, this);
        }

        public static Message Deserialise(NetworkStream networkStream)
        {
            return serialiseMessage.Deserialise(networkStream);
        }

        private void SetTextOfMessage(string messageText)
        {
            text = messageText;
            Log.Debug("Message text set: " + text);
        }

        private void SetTimeStampOfMessage()
        {
            messageTimeStamp = DateTime.Now;
            Log.Debug("Time stamp created: " + messageTimeStamp);
        }
    }
}