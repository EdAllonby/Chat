using System.Collections.Generic;
using SharedClasses;
using SharedClasses.Message;

namespace Server.MessageHandler
{
    /// <summary>
    /// Holds the link between an <see cref="IMessage"/> and their implementation of an <see cref="IMessageHandler"/> to be used by the Server.
    /// </summary>
    internal static class MessageHandlerRegistry
    {
        /// <summary>
        /// A dictionary of <see cref="IMessageHandler"/> implementations indexed by their relevant <see cref="MessageIdentifier"/> to be used by the Server.
        /// </summary>
        public static readonly IReadOnlyDictionary<MessageIdentifier, IMessageHandler>
            MessageHandlersIndexedByMessageIdentifier = new Dictionary<MessageIdentifier, IMessageHandler>
            {
                {MessageIdentifier.UserSnapshotRequest, new UserSnapshotRequestHandler()},
                {MessageIdentifier.ConversationSnapshotRequest, new ConversationSnapshotRequestHandler()},
                {MessageIdentifier.ParticipationSnapshotRequest, new ParticipationSnapshotRequestHandler()},
                {MessageIdentifier.ContributionRequest, new ContributionRequestHandler()},
                {MessageIdentifier.ClientDisconnection, new ClientDisconnectionHandler()},
                {MessageIdentifier.ParticipationRequest, new ParticipationRequestHandler()},
                {MessageIdentifier.ConversationRequest, new ConversationRequestHandler()},
                {MessageIdentifier.AvatarRequest, new AvatarRequestHandler()}
            };
    }
}