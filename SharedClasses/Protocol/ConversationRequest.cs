using System;

namespace SharedClasses.Protocol
{
    /// <summary>
    /// Packages <see cref="SenderID"/> and <see cref="ReceiverID"/> for the client to send to the server
    /// </summary>
    [Serializable]
    public sealed class ConversationRequest : IMessage
    {
        public ConversationRequest(int senderID, int receiverID)
        {
            SenderID = senderID;
            ReceiverID = receiverID;

            Identifier = MessageNumber.ConversationRequest;
        }

        public int SenderID { get; private set; }

        public int ReceiverID { get; private set; }

        public int Identifier { get; private set; }
    }
}