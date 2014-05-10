using System;
using System.Diagnostics.Contracts;
using SharedClasses.Domain;

namespace SharedClasses.Message
{
    /// <summary>
    /// Packages a <see cref="Contribution"/> without an Id for the Client to send to the Server
    /// </summary>
    [Serializable]
    public sealed class ConversationRequest : IMessage
    {
        public ConversationRequest(int clientUserId, int receiverId)
        {
            Contract.Requires(clientUserId > 0);
            Contract.Requires(receiverId > 0);

            Conversation = new Conversation(clientUserId, receiverId);

            Identifier = MessageNumber.ConversationRequest;
        }

        public Conversation Conversation { get; private set; }

        public int Identifier { get; private set; }
    }
}