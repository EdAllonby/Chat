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

        private static ITcpSendBehaviour serialiseMessage;

        private DateTime messageTimeStamp;

        public Message(string text)
        {
            CreateMessage(text);
            Log.Debug("Message created");
            SetSerialiseMethod(new BinaryFormat());
        }

        public string Text { get; private set; }

        public static void SetSerialiseMethod(ITcpSendBehaviour method)
        {
            serialiseMessage = method;
            Log.Debug("Serialise method set to: " + serialiseMessage.GetType());
        }

        public string GetMessage()
        {
            return Text + " sent at " + messageTimeStamp;
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
            serialiseMessage.Serialise(networkStream, this);
        }

        public static Message Deserialise(NetworkStream networkStream)
        {
            return serialiseMessage.Deserialise(networkStream);
        }

        private void SetTextOfMessage(string messageText)
        {
            Text = messageText;
            Log.Debug("Message text set: " + Text);
        }

        private void SetTimeStampOfMessage()
        {
            messageTimeStamp = DateTime.Now;
            Log.Debug("Time stamp created: " + messageTimeStamp);
        }
    }
}