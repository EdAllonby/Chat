using System;
using log4net;

namespace SharedClasses.Domain
{
    /// <summary>
    /// Used as the fundamental object needed to hold a text message and its timestamp
    /// </summary>
    [Serializable]
    public class Contribution
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (Contribution));

        public Contribution(string text)
        {
            CreateMessage(text);
            Log.Debug("Contribution created");
        }

        public DateTime MessageTimeStamp { get; private set; }
        public string Text { get; private set; }

        public override string ToString()
        {
            return Text + " @ " + MessageTimeStamp;
        }

        private void CreateMessage(string messageText)
        {
            SetTextOfMessage(messageText ?? String.Empty);
            SetTimeStampOfMessage();
        }

        private void SetTextOfMessage(string messageText)
        {
            Text = messageText;
            Log.Debug("Contribution text set: " + Text);
        }

        private void SetTimeStampOfMessage()
        {
            MessageTimeStamp = DateTime.Now;
            Log.Debug("Time stamp created: " + MessageTimeStamp);
        }
    }
}