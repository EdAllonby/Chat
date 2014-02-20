using System;

namespace Server
{
    [Serializable]
    public class Message
    {
        private string text;
        private DateTime messageTimeStamp;

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

        private void SetTextOfMessage(string messageText)
        {
            text = messageText;
        }

        private void SetTimeStampOfMessage()
        {
            messageTimeStamp = DateTime.Now;
        }

        public string GetText()
        {
            return text;
        }

        public DateTime GetTimeStamp()
        {
            return messageTimeStamp;
        }
    }
}