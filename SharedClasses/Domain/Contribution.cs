using System;
using System.Globalization;
using log4net;

namespace SharedClasses.Domain
{
    /// <summary>
    /// Used as the fundamental object needed to hold a text message and its timestamp
    /// </summary>
    [Serializable]
    public sealed class Contribution
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (Contribution));

        public Contribution(int id, User contributor, string text, Conversation conversation)
        {
            ID = id;
            Contributor = contributor;
            CreateMessage(text);
            Conversation = conversation;
            Log.Debug("Contribution created");
        }

        /// <summary>
        /// The Conversation this Contribution belongs to
        /// </summary>
        public Conversation Conversation { get; private set; }

        /// <summary>
        /// The Unique ID of this Contribution
        /// </summary>
        public int ID { get; private set; }

        /// <summary>
        /// The User who sent this Contribution message
        /// </summary>
        public User Contributor { get; set; }

        /// <summary>
        /// The time that the message was created
        /// TODO: Create the time in the server, as user's PCs can have different/incorrect local times
        /// </summary>
        public DateTime MessageTimeStamp { get; private set; }

        /// <summary>
        /// The contribution text
        /// </summary>
        public string Text { get; private set; }

        public string SenderInformation
        {
            get { return Contributor.UserName + " sent message at: " + MessageTimeStamp.ToString("HH:mm:ss dd/MM/yyyy", new CultureInfo("en-GB")); }
        }

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