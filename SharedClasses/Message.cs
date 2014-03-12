using System;
using System.IO;
using SharedClasses.Serialisation;

namespace SharedClasses
{
    [Serializable]
    public class Message
    {
        private static readonly ITcpSendBehaviour SerialiseMessage = new BinaryFormat();

        public string text { get; private set; }
        public DateTime messageTimeStamp;

        public Message(string text)
        {
            CreateMessage(text);
        }

        private void CreateMessage(string messageText)
        {
            SetTextOfMessage(messageText);

            if (text != null)
            {
                SetTimeStampOfMessage();
            }
        }

        public void Serialise(Stream newtworkStream)
        {
            SerialiseMessage.Serialise(newtworkStream, this);
        }

        public static Message Deserialise(Stream newtworkStream)
        {
            return SerialiseMessage.Deserialise(newtworkStream);
        }

        private void SetTextOfMessage(string messageText)
        {
            text = messageText;
        }

        private void SetTimeStampOfMessage()
        {
            messageTimeStamp = DateTime.Now;
        }
    }
}