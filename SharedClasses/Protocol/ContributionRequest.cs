using System;

namespace SharedClasses.Protocol
{
    /// <summary>
    /// Packages  <see cref="ConversationID"/>, <see cref="SenderID"/> and <see cref="Message"/> for the client to send to the server
    /// </summary>
    [Serializable]
    public class ContributionRequest : IMessage
    {
        public ContributionRequest(int conversationID, int senderID, string message)
        {
            ConversationID = conversationID;
            SenderID = senderID;
            Message = message;
            Identifier = MessageNumber.ContributionRequest;
        }

        public int ConversationID { get; private set; }

        public int SenderID { get; private set; }

        public string Message { get; private set; }

        public int Identifier { get; private set; }
    }
}