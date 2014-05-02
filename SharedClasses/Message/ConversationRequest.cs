using System;
using SharedClasses.Domain;

namespace SharedClasses.Protocol
{
    /// <summary>
    /// Packages a <see cref="Contribution"/> without an Id for the Client to send to the Server
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