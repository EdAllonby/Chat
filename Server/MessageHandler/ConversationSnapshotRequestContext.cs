using System.Collections.Generic;
using SharedClasses;
using SharedClasses.Domain;

namespace Server.MessageHandler
{
    /// <summary>
    /// Holds a reference to the necessary objects a <see cref="ConversationSnapshotRequestHandler"/> needs. 
    /// </summary>
    internal sealed class ConversationSnapshotRequestContext : IMessageContext
    {
        private readonly Dictionary<int, ClientHandler> clientHandlersIndexedByUserId;
        private readonly ConversationRepository conversationRepository;
        private readonly ParticipationRepository participationRepository;

        public ConversationSnapshotRequestContext(ParticipationRepository participationRepository,
            ConversationRepository conversationRepository,
            Dictionary<int, ClientHandler> clientHandlersIndexedByUserId)
        {
            this.participationRepository = participationRepository;
            this.conversationRepository = conversationRepository;
            this.clientHandlersIndexedByUserId = clientHandlersIndexedByUserId;
        }

        public ParticipationRepository ParticipationRepository
        {
            get { return participationRepository; }
        }

        public ConversationRepository ConversationRepository
        {
            get { return conversationRepository; }
        }

        public Dictionary<int, ClientHandler> ClientHandlersIndexedByUserId
        {
            get { return clientHandlersIndexedByUserId; }
        }
    }
}