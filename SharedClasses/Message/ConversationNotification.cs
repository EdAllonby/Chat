using System;
using System.Diagnostics.Contracts;
using SharedClasses.Domain;

namespace SharedClasses.Message
{
    [Serializable]
    public class ConversationNotification : IMessage
    {
        public ConversationNotification(Conversation conversation, NotificationType notificationType)
        {
            Contract.Requires(conversation != null);
            Contract.Requires(conversation.Id > 0);

            Conversation = conversation;
            NotificationType = notificationType;
        }

        public Conversation Conversation { get; private set; }

        public NotificationType NotificationType { get; private set; }

        public MessageIdentifier MessageIdentifier
        {
            get { return MessageIdentifier.ConversationNotification; }
        }
    }
}