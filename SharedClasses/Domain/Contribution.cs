using System;
using System.Globalization;
using log4net;

namespace SharedClasses.Domain
{
    /// <summary>
    /// Used as the fundamental object needed to hold a text message and its timestamp
    /// </summary>
    [Serializable]
    public sealed class Contribution : IEquatable<Contribution>
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (Contribution));

        // Immutable domain entity class
        private readonly int contributorUserId;
        private readonly int conversationId;

        public Contribution(int contributionId, int contributorUserId, string text, int conversationId)
        {
            ContributionId = contributionId;
            this.contributorUserId = contributorUserId;
            CreateMessage(text);
            this.conversationId = conversationId;
        }

        /// <summary>
        /// The Conversation ID this Contribution belongs to
        /// </summary>
        public int ConversationId
        {
            get { return conversationId; }
        }

        /// <summary>
        /// The Unique ID of this Contribution
        /// </summary>
        public int ContributionId { get; private set; }

        /// <summary>
        /// The User who sent this Contribution message
        /// </summary>
        public int ContributorUserId
        {
            get { return contributorUserId; }
        }

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
            get { return ContributorUserId + " sent message at: " + MessageTimeStamp.ToString("HH:mm:ss dd/MM/yyyy", new CultureInfo("en-GB")); }
        }

        public bool Equals(Contribution other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return ContributionId == other.ContributionId;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is Contribution && Equals((Contribution) obj);
        }

        public override int GetHashCode()
        {
            return ContributionId;
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