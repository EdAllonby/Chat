using System;
using System.Collections.Generic;

namespace SharedClasses.Message
{
    /// <summary>
    /// Packages a List of user Ids that should be part of a conversation
    /// </summary>
    [Serializable]
    public sealed class ConversationRequest : IMessage
    {
        public ConversationRequest(List<int> participantIds)
        {
            ParticipantIds = participantIds;

            Identifier = MessageNumber.ConversationRequest;
        }

        public List<int> ParticipantIds { get; private set; }

        public MessageNumber Identifier { get; private set; }
    }
}