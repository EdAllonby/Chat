using System.Collections.Generic;
using SharedClasses;

namespace Server.MessageHandler
{
    /// <summary>
    /// Holds the contexts the Server <see cref="IMessageHandler"/>s need to function.
    /// </summary>
    internal sealed class ServerContextRegistry
    {
        /// <summary>
        /// Contains an immutable dictionary of <see cref="IMessageContext"/>s to be used with an <see cref="IMessageHandler"/>.
        /// </summary>
        public readonly IReadOnlyDictionary<MessageIdentifier, IMessageContext> MessageHandlersIndexedByMessageIdentifier;

        /// <summary>
        /// Creates a <see cref="IMessageContext"/> object for each <see cref="MessageIdentifier"/>.
        /// </summary>
        /// <param name="repositoryManager">Holds the repositories that each <see cref="IMessageContext"/> needs.</param>
        /// <param name="clientHandlersIndexedByUserId">Holds the currently connected clients that each <see cref="IMessageContext"/> needs.</param>
        /// <param name="entityIdAllocatorFactory">Holds the entity ID Generator to create new domain objects.</param>
        public ServerContextRegistry(RepositoryManager repositoryManager,
            Dictionary<int, ClientHandler> clientHandlersIndexedByUserId,
            EntityIdAllocatorFactory entityIdAllocatorFactory)
        {
            MessageHandlersIndexedByMessageIdentifier = new Dictionary<MessageIdentifier, IMessageContext>
            {
                {
                    MessageIdentifier.UserSnapshotRequest,
                    new UserSnapshotRequestContext(repositoryManager.UserRepository, clientHandlersIndexedByUserId)
                },
                {
                    MessageIdentifier.ConversationSnapshotRequest,
                    new ConversationSnapshotRequestContext(repositoryManager.ParticipationRepository,
                        repositoryManager.ConversationRepository,
                        clientHandlersIndexedByUserId)
                },
                {
                    MessageIdentifier.ParticipationSnapshotRequest,
                    new ParticipationSnapshotRequestContext(repositoryManager.ParticipationRepository,
                        clientHandlersIndexedByUserId)
                },
                {
                    MessageIdentifier.ContributionRequest,
                    new ContributionRequestContext(entityIdAllocatorFactory, repositoryManager.ConversationRepository)
                },
                {
                    MessageIdentifier.ClientDisconnection,
                    new ClientDisconnectionContext(clientHandlersIndexedByUserId, repositoryManager.UserRepository)
                },
                {
                    MessageIdentifier.ParticipationRequest,
                    new ParticipationRequestContext(repositoryManager.ParticipationRepository, entityIdAllocatorFactory)
                },
                {
                    MessageIdentifier.NewConversationRequest,
                    new NewConversationRequestContext(repositoryManager.ParticipationRepository,
                        repositoryManager.ConversationRepository, entityIdAllocatorFactory)
                },
            };
        }
    }
}