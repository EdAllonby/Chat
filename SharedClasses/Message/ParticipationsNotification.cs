using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using SharedClasses.Domain;

namespace SharedClasses.Message
{
    /// <summary>
    /// Packages <see cref="Participation"/>s related by conversation Id for the Server to send to the Client
    /// </summary>
    [Serializable]
    public sealed class ParticipationsNotification : IMessage
    {
        public ParticipationsNotification(List<int> participantIds, int conversationId)
        {
            Contract.Requires(participantIds != null);
            Contract.Requires(conversationId > 0);

            ParticipantIds = participantIds;
            ConversationId = conversationId;
        }

        public List<int> ParticipantIds { get; private set; }

        public int ConversationId { get; private set; }

        public MessageIdentifier MessageIdentifier
        {
            get { return MessageIdentifier.ParticipationsNotification; }
        }
    }
}