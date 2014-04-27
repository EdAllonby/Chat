using System;
using SharedClasses.Domain;

namespace SharedClasses.Protocol
{
    /// <summary>
    /// Packages <see cref="SenderID"/> and <see cref="ReceiverID"/> for the client to send to the server
    /// </summary>
    [Serializable]
    public sealed class ConversationRequest : IMessage
    {
        public ConversationRequest(Conversation conversation)
        {
            Conversation = conversation;

            Identifier = MessageNumber.ConversationRequest;
        }

        public Conversation Conversation { get; private set; }

        public int Identifier { get; private set; }
    }
}