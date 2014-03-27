using System;
using log4net;

namespace SharedClasses.Domain
{
    [Serializable]
    public class Contribution
    {
        public int MessageType;

        private static readonly ILog Log = LogManager.GetLogger(typeof (Contribution));

        private DateTime messageTimeStamp;
        private string text;

        public Contribution(string text)
        {
            CreateMessage(text);
            Log.Debug("Contribution created");
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