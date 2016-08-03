using System.Collections.Generic;
using SharedClasses;
using SharedClasses.Message;

namespace Server.MessageHandler
{
    /// <summary>
    /// Holds the link between an <see cref="IMessage" /> and their implementation of an <see cref="MessageHandler" /> to be
    /// used by the Server.
    /// </summary>
    internal class MessageHandlerRegistry
    {
        /// <summary>
        /// A dictionary of <see cref="IMessageHandler" /> implementations indexed by their relevant
        /// <see cref="MessageIdentifier" /> to be used by the Client.
        /// </summary>
        public readonly IReadOnlyDictionary<MessageIdentifier, IMessageHandler> MessageHandlersIndexedByMessageIdentifier;


        public MessageHandlerRegistry(IServiceRegistry serviceRegistry)
        {
            MessageHandlersIndexedByMessageIdentifier = new Dictionary<MessageIdentifier, IMessageHandler>
            {
                { MessageIdentifier.UserSnapshotRequest, new UserSnapshotRequestHandler(serviceRegistry) },
                { MessageIdentifier.ConversationSnapshotRequest, new ConversationSnapshotRequestHandler(serviceRegistry) },
                { MessageIdentifier.ParticipationSnapshotRequest, new ParticipationSnapshotRequestHandler(serviceRegistry) },
                { MessageIdentifier.ContributionRequest, new ContributionRequestHandler(serviceRegistry) },
                { MessageIdentifier.ClientDisconnection, new ClientDisconnectionHandler(serviceRegistry) },
                { MessageIdentifier.ParticipationRequest, new ParticipationRequestHandler(serviceRegistry) },
                { MessageIdentifier.ConversationRequest, new ConversationRequestHandler(serviceRegistry) },
                { MessageIdentifier.AvatarRequest, new AvatarRequestHandler(serviceRegistry) },
                { MessageIdentifier.UserTypingRequest, new UserTypingRequestHandler(serviceRegistry) }
            };
        }
    }
}