﻿using System;
using SharedClasses.Domain;
using SharedClasses.Message;

namespace ChatClient.Services.MessageHandler
{
    /// <summary>
    /// Handles a <see cref="ConversationSnapshot"/> the Client received.
    /// </summary>
    internal sealed class ConversationSnapshotHandler : IClientMessageHandler
    {
        public void HandleMessage(IMessage message, IClientMessageContext context)
        {
            var conversationSnapshot = (ConversationSnapshot) message;

            foreach (Conversation conversation in conversationSnapshot.Conversations)
            {
                context.RepositoryManager.ConversationRepository.AddEntity(conversation);
            }

            OnConversationBootstrapCompleted();
        }

        public event EventHandler ConversationBootstrapCompleted;

        private void OnConversationBootstrapCompleted()
        {
            EventHandler handler = ConversationBootstrapCompleted;
            if (handler != null) handler(this, EventArgs.Empty);
        }
    }
}