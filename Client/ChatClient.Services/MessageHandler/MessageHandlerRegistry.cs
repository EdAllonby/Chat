using System.Collections.Generic;
using SharedClasses;
using SharedClasses.Message;

namespace ChatClient.Services.MessageHandler
{
    /// <summary>
    /// Holds the link between an <see cref="IMessage" /> and their implementation of an <see cref="IMessageHandler" />
    /// to be used by the Client.
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
                { MessageIdentifier.ContributionNotification, new ContributionNotificationHandler(serviceRegistry) },
                { MessageIdentifier.UserNotification, new UserNotificationHandler(serviceRegistry) },
                { MessageIdentifier.ConversationNotification, new ConversationNotificationHandler(serviceRegistry) },
                { MessageIdentifier.ParticipationNotification, new ParticipationNotificationHandler(serviceRegistry) },
                { MessageIdentifier.ConnectionStatusNotification, new ConnectionStatusNotificationHandler(serviceRegistry) },
                { MessageIdentifier.AvatarNotification, new AvatarNotificationHandler(serviceRegistry) },
                { MessageIdentifier.UserSnapshot, new UserSnapshotHandler(serviceRegistry) },
                { MessageIdentifier.ConversationSnapshot, new ConversationSnapshotHandler(serviceRegistry) },
                { MessageIdentifier.ParticipationSnapshot, new ParticipationSnapshotHandler(serviceRegistry) },
                { MessageIdentifier.UserTypingNotification, new UserTypingNotificationHandler(serviceRegistry) }
            };
        }

        public IReadOnlyCollection<IBootstrapper> Bootstrappers => new List<IBootstrapper>
        {
            (IBootstrapper) MessageHandlersIndexedByMessageIdentifier[MessageIdentifier.ParticipationSnapshot],
            (IBootstrapper) MessageHandlersIndexedByMessageIdentifier[MessageIdentifier.UserSnapshot],
            (IBootstrapper) MessageHandlersIndexedByMessageIdentifier[MessageIdentifier.ConversationSnapshot]
        };
    }
}