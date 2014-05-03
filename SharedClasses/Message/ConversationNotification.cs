using System;
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
            Conversation = conversation;
            Identifier = MessageNumber.ConversationNotification;
        }

        public Conversation Conversation { get; private set; }

        public int Identifier { get; private set; }
    }
}