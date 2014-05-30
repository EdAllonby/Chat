using System;
using System.Collections.Generic;

namespace SharedClasses.Message
{
    /// <summary>
    /// Packages a List of user Ids that should be part of a new conversation
    /// </summary>
    [Serializable]
    public sealed class NewConversationRequest : IMessage
    {
        public NewConversationRequest(List<int> participantIds)
        {
            ParticipantIds = participantIds;
        }

        public List<int> ParticipantIds { get; private set; }

        public MessageIdentifier MessageIdentifier
        {
            get { return MessageIdentifier.NewConversationRequest; }
        }
    }
}