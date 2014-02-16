using System;

namespace Server
{
    public class Message
    {
        private readonly string text;
        private readonly DateTime timeStamp;

        public Message(string text, DateTime timeStamp)
        {
            this.text = text;
            this.timeStamp = timeStamp;
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