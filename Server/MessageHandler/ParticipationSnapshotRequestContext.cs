using System.Collections.Generic;
using SharedClasses;
using SharedClasses.Domain;

namespace Server.MessageHandler
{
    /// <summary>
    /// Holds a reference to the necessary objects a <see cref="ParticipationSnapshotRequestHandler"/> needs. 
    /// </summary>
    internal sealed class ParticipationSnapshotRequestContext : IMessageContext
    {
        private readonly IDictionary<int, ClientHandler> clientHandlersIndexedByUserId;
        private readonly ParticipationRepository participationRepository;

        public ParticipationSnapshotRequestContext(ParticipationRepository participationRepository,
            IDictionary<int, ClientHandler> clientHandlersIndexedByUserId)
        {
            this.participationRepository = participationRepository;
            this.clientHandlersIndexedByUserId = clientHandlersIndexedByUserId;
        }

        public ParticipationRepository ParticipationRepository
        {
            get { return participationRepository; }
        }

        public IDictionary<int, ClientHandler> ClientHandlersIndexedByUserId
        {
            get { return clientHandlersIndexedByUserId; }
        }
    }
}