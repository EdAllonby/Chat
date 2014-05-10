﻿using System.Net.Sockets;
using log4net;
using SharedClasses.Domain;
using SharedClasses.Message;

namespace SharedClasses.Serialiser
{
    /// <summary>
    /// Used to serialise and deserialise a <see cref="ConversationRequest" /> message
    /// Uses <see cref="ConversationSerialiser" /> for its underlying serialiser
    /// </summary>
    internal sealed class ConversationRequestSerialiser : ISerialiser<ConversationRequest>
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (ConversationRequestSerialiser));

        private readonly ConversationSerialiser conversationSerialiser = new ConversationSerialiser();

        private readonly MessageIdentifierSerialiser messageIdentifierSerialiser = new MessageIdentifierSerialiser();

        public void Serialise(ConversationRequest message, NetworkStream stream)
        {
            messageIdentifierSerialiser.SerialiseMessageIdentifier(MessageNumber.ConversationRequest, stream);

            Log.Debug("Waiting for conversation request message to serialise");
            conversationSerialiser.Serialise(message.Conversation, stream);
            Log.Info("Conversation request message serialised");
        }

        public void Serialise(IMessage message, NetworkStream stream)
        {
            Serialise((ConversationRequest) message, stream);
        }

        public IMessage Deserialise(NetworkStream networkStream)
        {
            Conversation conversation = conversationSerialiser.Deserialise(networkStream);
            var request = new ConversationRequest(conversation.FirstParticipantUserId, conversation.SecondParticipantUserId);
            Log.Info("Conversation request message deserialised");
            return request;
        }
    }
}