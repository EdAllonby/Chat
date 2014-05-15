using System;
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
        public ConversationNotification(Conversation conversation)
        {
            Contract.Requires(conversation != null);
            Contract.Requires(conversation.ConversationId > 0);

            Conversation = conversation;
            Identifier = MessageNumber.ConversationNotification;
        }

        public Conversation Conversation { get; private set; }

        public MessageNumber Identifier { get; private set; }
    }
}