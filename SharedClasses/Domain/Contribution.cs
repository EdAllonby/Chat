using System;
using log4net;

namespace SharedClasses.Domain
{
    /// <summary>
    ///     Used as the fundamental object needed to hold a text message and its timestamp
    /// </summary>
    [Serializable]
    public class Contribution
    {
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