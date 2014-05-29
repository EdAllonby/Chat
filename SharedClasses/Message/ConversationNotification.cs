using System;
using System.Diagnostics.Contracts;
using SharedClasses.Domain;

namespace SharedClasses.Message
{
    [Serializable]
    public class ConversationNotification : IMessage
    {
        public ConversationNotification(Conversation conversation)
        {
            Contract.Requires(conversation != null);
            Contract.Requires(conversation.ConversationId > 0);

            Conversation = conversation;
        }

        public Conversation Conversation { get; private set; }

        public MessageNumber Identifier
        {
            get { return MessageNumber.ConversationNotification; }
        }
    }
}