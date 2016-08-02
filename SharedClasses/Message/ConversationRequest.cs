using System;
using System.Collections.Generic;

namespace SharedClasses.Message
{
    /// <summary>
    /// Packages a List of user Ids that should be part of a new conversation
    /// </summary>
    [Serializable]
    public sealed class ConversationRequest : IMessage
    {
        public ConversationRequest(List<int> userIds)
        {
            UserIds = userIds;
        }

        public List<int> UserIds { get; private set; }

        public MessageIdentifier MessageIdentifier => MessageIdentifier.ConversationRequest;
    }
}