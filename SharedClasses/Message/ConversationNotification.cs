using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using SharedClasses.Domain;

namespace SharedClasses.Message
{
    /// <summary>
    /// Packages a <see cref="Conversation"/> for the Server to send to the Client
    /// </summary>
    [Serializable]
    public sealed class ConversationNotification : IMessage
    {
        public ConversationNotification(List<int> participantIds, int conversationId)
        {
            Contract.Requires(participantIds != null);
            Contract.Requires(conversationId > 0);

            ParticipantIds = participantIds;
            ConversationId = conversationId;
        }

        public List<int> ParticipantIds { get; private set; }

        public int ConversationId { get; private set; }

        public MessageNumber Identifier
        {
            get { return MessageNumber.ConversationNotification; }
        }
    }
}