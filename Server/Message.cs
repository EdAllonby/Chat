using System;

namespace Server
{
    public class Message
    {
        private string text;
        private DateTime timeStamp;

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