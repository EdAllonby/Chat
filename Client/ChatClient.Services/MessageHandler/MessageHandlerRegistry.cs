using System.Collections.Generic;
using SharedClasses;
using SharedClasses.Message;

namespace ChatClient.Services.MessageHandler
{
    /// <summary>
    /// Holds the link between an <see cref="IMessage" /> and their implementation of an <see cref="IClientMessageHandler" />
    /// to be used by the Client.
    /// </summary>
    internal static class MessageHandlerRegistry
    {
        /// <summary>
        /// A dictionary of <see cref="IClientMessageHandler" /> implementations indexed by their relevant
        /// <see cref="MessageIdentifier" /> to be used by the Client.
        /// </summary>
        public static readonly IReadOnlyDictionary<MessageIdentifier, IMessageHandler>
            MessageHandlersIndexedByMessageIdentifier = new Dictionary<MessageIdentifier, IMessageHandler>
            {
                { MessageIdentifier.ContributionNotification, new ContributionNotificationHandler() },
                { MessageIdentifier.UserNotification, new UserNotificationHandler() },
                { MessageIdentifier.ConversationNotification, new ConversationNotificationHandler() },
                { MessageIdentifier.ParticipationNotification, new ParticipationNotificationHandler() },
                { MessageIdentifier.ConnectionStatusNotification, new ConnectionStatusNotificationHandler() },
                { MessageIdentifier.AvatarNotification, new AvatarNotificationHandler() },
                { MessageIdentifier.UserSnapshot, new UserSnapshotHandler() },
                { MessageIdentifier.ConversationSnapshot, new ConversationSnapshotHandler() },
                { MessageIdentifier.ParticipationSnapshot, new ParticipationSnapshotHandler() },
                { MessageIdentifier.UserTypingNotification, new UserTypingNotificationHandler() }
            };
    }
}