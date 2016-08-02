using System;
using System.Collections.Generic;
using SharedClasses.Domain;

namespace SharedClasses.Message
{
    /// <summary>
    /// Sends a list of conversations that is associated with the <see cref="User" />
    /// </summary>
    [Serializable]
    public sealed class ConversationSnapshot : IMessage
    {
        public ConversationSnapshot(IEnumerable<Conversation> conversations)
        {
            Conversations = conversations;
        }

        public IEnumerable<Conversation> Conversations { get; private set; }

        public MessageIdentifier MessageIdentifier => MessageIdentifier.ConversationSnapshot;
    }
}