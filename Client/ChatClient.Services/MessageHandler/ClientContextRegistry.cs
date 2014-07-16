using System.Collections.Generic;
using SharedClasses;

namespace ChatClient.Services.MessageHandler
{
    /// <summary>
    /// Holds the contexts the Client <see cref="IMessageHandler"/>s need to function.
    /// </summary>
    internal sealed class ClientContextRegistry
    {
        /// <summary>
        /// Contains an immutable dictionary of <see cref="IMessageContext"/>s to be used with an <see cref="IMessageHandler"/>.
        /// </summary>
        public readonly IReadOnlyDictionary<MessageIdentifier, IMessageContext> MessageHandlersIndexedByMessageIdentifier;

        /// <summary>
        /// Creates a <see cref="IMessageContext"/> object for each <see cref="MessageIdentifier"/>.
        /// </summary>
        /// <param name="repositoryManager">Holds the repositories that each <see cref="IMessageContext"/> needs.</param>
        public ClientContextRegistry(RepositoryManager repositoryManager)
        {
            MessageHandlersIndexedByMessageIdentifier = new Dictionary<MessageIdentifier, IMessageContext>
            {
                {
                    MessageIdentifier.ContributionNotification,
                    new ContributionNotificationContext(repositoryManager.ConversationRepository)
                },
                {
                    MessageIdentifier.UserNotification,
                    new UserNotificationContext(repositoryManager.UserRepository)
                },
                {
                    MessageIdentifier.ConversationNotification,
                    new ConversationNotificationContext(repositoryManager.ConversationRepository)
                },
                {
                    MessageIdentifier.ParticipationNotification,
                    new ParticipationNotificationContext(repositoryManager.ParticipationRepository)
                },
                {
                    MessageIdentifier.ConnectionStatusNotification,
                    new ConnectionStatusNotificationContext(repositoryManager.UserRepository)
                },
                {
                    MessageIdentifier.AvatarNotification,
                    new AvatarNotificationContext(repositoryManager.UserRepository)
                },
                {
                    MessageIdentifier.UserSnapshot,
                    new UserSnapshotContext(repositoryManager.UserRepository)
                },
                {
                    MessageIdentifier.ConversationSnapshot,
                    new ConversationSnapshotContext(repositoryManager.ConversationRepository)
                },
                {
                    MessageIdentifier.ParticipationSnapshot,
                    new ParticipationSnapshotContext(repositoryManager.ParticipationRepository)
                }
            };
        }
    }
}