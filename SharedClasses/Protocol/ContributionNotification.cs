using System;
using SharedClasses.Domain;

namespace SharedClasses.Protocol
{
    /// <summary>
    /// Packages  <see cref="ConversationID"/>, <see cref="SenderID"/> and <see cref="Message"/> for the Server to send to the Client
    /// </summary>
    [Serializable]
    public class ContributionNotification : IMessage
    {
        public ContributionNotification(int contributionID, int conversationID, int senderID, string message)
        {
            ContributionID = contributionID;
            ConversationID = conversationID;
            SenderID = senderID;
            Message = message;
            Identifier = MessageNumber.ContributionNotification;
        }

        public int ContributionID { get; private set; }

        public int ConversationID { get; private set; }

        public int SenderID { get; private set; }

        public string Message { get; private set; }

        public Contribution Contribution { get; private set; }

        public int Identifier { get; private set; }
    }
}