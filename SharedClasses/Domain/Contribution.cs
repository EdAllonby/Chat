using System;
using System.Net.Sockets;
using log4net;
using SharedClasses.Protocol;

namespace SharedClasses.Domain
{
    [Serializable]
    public class Contribution
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (Contribution));

        private static ITcpSendBehaviour serialiseMessage = new BinaryFormat();

        private string text;
        private DateTime messageTimeStamp;

        public Contribution(string text)
        {
            CreateMessage(text);
            Log.Debug("Contribution created");
            SetSerialiseMethod(new BinaryFormat());
        }

        public static void SetSerialiseMethod(ITcpSendBehaviour method)
        {
            serialiseMessage = method;
            Log.Debug("Serialise method set to: " + serialiseMessage.GetType());
        }

        public string GetMessage()
        {
            return text + " @ " + messageTimeStamp;
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

        public static Contribution Deserialise(NetworkStream networkStream)
        {
            return serialiseMessage.Deserialise(networkStream);
        }

        private void SetTextOfMessage(string messageText)
        {
            text = messageText;
            Log.Debug("Contribution text set: " + text);
        }

        private void SetTimeStampOfMessage()
        {
            messageTimeStamp = DateTime.Now;
            Log.Debug("Time stamp created: " + messageTimeStamp);
        }
    }
}