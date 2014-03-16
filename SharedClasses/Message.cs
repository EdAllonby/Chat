using System;
using System.Net.Sockets;
using SharedClasses.Serialisation;

namespace SharedClasses
{
    [Serializable]
    public class Message
    {
        private static readonly log4net.ILog Log =
            log4net.LogManager.GetLogger(typeof (Message));

        private static readonly ITcpSendBehaviour SerialiseMessage = new BinaryFormat();

        public string Text { get; private set; }
        public DateTime MessageTimeStamp;

        public Message(string text)
        {
            CreateMessage(text);
            Log.Debug("Message created");
        }

        private void CreateMessage(string messageText)
        {
            SetTextOfMessage(messageText);

            if (Text != null)
            {
                SetTimeStampOfMessage();
            }
        }

        public void Serialise(NetworkStream networkStream)
        {
            SerialiseMessage.Serialise(networkStream, this);
        }

        public static Message Deserialise(NetworkStream networkStream)
        {
            return SerialiseMessage.Deserialise(networkStream);
        }

        private void SetTextOfMessage(string messageText)
        {
            Text = messageText;
            Log.Debug("Message text set: " + Text);
        }

        private void SetTimeStampOfMessage()
        {
            MessageTimeStamp = DateTime.Now;
            Log.Debug("Time stamp created: " + MessageTimeStamp);
        }
    }
}