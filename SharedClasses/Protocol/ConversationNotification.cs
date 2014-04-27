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
            Conversation = conversation;
            Identifier = MessageNumber.ConversationNotification;
        }

        public Conversation Conversation { get; private set; }
        public int Identifier { get; private set; }
    }
}