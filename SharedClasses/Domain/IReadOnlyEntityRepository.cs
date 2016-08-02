using System;
using System.Collections.Generic;

namespace SharedClasses.Domain
{
    /// <summary>
    /// Exposes only the readonly functionality of an Entity Repository.
    /// </summary>
    /// <typeparam name="T">The entity type that this readonly repository holds.</typeparam>
    public interface IReadOnlyEntityRepository<T> where T : IEntity
    {
        /// <summary>
        /// Gets raised when an entity in the repository has been added.
        /// </summary>
        event EventHandler<EntityChangedEventArgs<T>> EntityAdded;

        /// <summary>
        /// Gets raised when an entity in the repository has been updated.
        /// </summary>
        event EventHandler<EntityChangedEventArgs<T>> EntityUpdated;

        /// <summary>
        /// Gets raised when an entity in the repository has been removed.
        /// </summary>
        event EventHandler<EntityChangedEventArgs<T>> EntityRemoved;

        /// <summary>
        /// Retrieves an <see cref="IEntity" /> entity from the repository.
        /// </summary>
        /// <param name="entityId">The <see cref="IEntity" /> entity Id to find.</param>
        /// <returns>The <see cref="IEntity" /> which matches the ID. If no <see cref="IEntity" /> is found, return null.</returns>
        T FindEntityById(int entityId);

        /// <summary>
        /// Retrieves all <see cref="IEntity" />s from the repository.
        /// </summary>
        /// <returns>A collection of all <see cref="IEntity" />s in the repository.</returns>
        IEnumerable<T> GetAllEntities();
    }
}