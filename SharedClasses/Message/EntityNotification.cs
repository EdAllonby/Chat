using SharedClasses.Domain;

namespace SharedClasses.Message
{
    /// <summary>
    /// Packages an <see cref="Entity" /> with a <see cref="NotificationType" />.
    /// Used to notify clients when an <see cref="Entity" /> of type <see cref="TEntity" /> is changed Server side.
    /// </summary>
    /// <typeparam name="TEntity">The <see cref="Entity" /> changed.</typeparam>
    public sealed class EntityNotification<TEntity> : IMessage where TEntity : IEntity
    {
        private readonly NotificationMessageIdentifierFactory notificationMessageIdentifierFactory = new NotificationMessageIdentifierFactory();

        /// <summary>
        /// Create a new notification object for <see cref="TEntity" />.
        /// </summary>
        /// <param name="entity">The <see cref="TEntity" /> changed.</param>
        /// <param name="notificationType">What change the <see cref="TEntity" /> <see cref="Entity" /> made.</param>
        public EntityNotification(TEntity entity, NotificationType notificationType)
        {
            Entity = entity;
            NotificationType = notificationType;
        }

        /// <summary>
        /// What change the <see cref="TEntity" /> <see cref="Entity" /> made.
        /// </summary>
        public NotificationType NotificationType { get; private set; }

        /// <summary>
        /// The <see cref="TEntity" /> <see cref="Entity" /> changed.
        /// </summary>
        public TEntity Entity { get; private set; }

        /// <summary>
        /// What the message identifier is.
        /// </summary>
        public MessageIdentifier MessageIdentifier => notificationMessageIdentifierFactory.GetIdentifierBySnapshotType(typeof(TEntity));
    }
}