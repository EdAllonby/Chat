using System;
using SharedClasses.Domain;
using SharedClasses.Message;

namespace SharedClasses
{
    /// <summary>
    /// Signifies a entity change in a <see cref="EntityRepository{T}" />.
    /// </summary>
    /// <typeparam name="T">The entity that has been changed in the repository.</typeparam>
    public sealed class EntityChangedEventArgs<T> : EventArgs where T : IEntity
    {
        /// <summary>
        /// Signifies either an entity creation or entity deletion. When an entity is deleted, <see cref="PreviousEntity" /> is set
        /// to the current entity.
        /// </summary>
        /// <param name="entity">The entity that is created or deleted.</param>
        /// <param name="notificationType">What is happening to the entity in the repository.</param>
        public EntityChangedEventArgs(T entity, NotificationType notificationType)
        {
            Entity = entity;
            if (notificationType == NotificationType.Delete)
            {
                PreviousEntity = entity;
            }
        }

        /// <summary>
        /// Signifies an entity being updated in the <see cref="EntityRepository{T}" />.
        /// </summary>
        /// <param name="entity">The entity being updated.</param>
        /// <param name="previousEntity">The entity state before being updated.</param>
        public EntityChangedEventArgs(T entity, T previousEntity)
        {
            Entity = entity;
            PreviousEntity = previousEntity;

            NotificationType = NotificationType.Update;
        }

        /// <summary>
        /// The action being applied to the entity.
        /// </summary>
        public NotificationType NotificationType { get; private set; }

        /// <summary>
        /// The new entity.
        /// </summary>
        public T Entity { get; private set; }

        /// <summary>
        /// The previous state of the entity.
        /// </summary>
        public T PreviousEntity { get; private set; }
    }
}