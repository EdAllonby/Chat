using System;

namespace Server
{
    [Serializable]
    public class Message
    {
        private readonly string text;
        private readonly DateTime timeStamp;

        public Message(string text)
        {
            this.text = text;

            DateTime time = DateTime.Now;
            timeStamp = time;
        }

        public string GetText()
        {
            return text;
        }

        public DateTime GetTimeStamp()
        {
            return timeStamp;
        }
    }
}