using System;
using SharedClasses.Domain;

namespace SharedClasses.Protocol
{
    /// <summary>
    /// Transforms a <see cref="ConversationRequest"/> for the server to send to a client
    /// </summary>
    [Serializable]
    public sealed class ConversationNotification : IMessage
    {
        public ConversationNotification(Conversation conversation)
        {
            SenderID = conversation.FirstParticipant.ID;
            ReceiverID = conversation.SecondParticipant.ID;
            ConversationID = conversation.ID;

            Identifier = MessageNumber.ConversationNotification;
        }

        /// <summary>
        /// The Sender's Unique User ID
        /// </summary>
        public int SenderID { get; private set; }

        /// <summary>
        /// The Receiver's Unique User ID
        /// </summary>
        public int ReceiverID { get; private set; }

        /// <summary>
        /// The Conversation's Unique ID
        /// </summary>
        public int ConversationID { get; private set; }

        /// <summary>
        /// This particular message's identifier
        /// </summary>
        public int Identifier { get; private set; }
    }
}