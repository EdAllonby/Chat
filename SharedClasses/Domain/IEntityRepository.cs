using System;

namespace SharedClasses.Domain
{
    /// <summary>
    /// Non-generic entity repository interface.
    /// </summary>
    public interface IEntityRepository
    {
        /// <summary>
        /// Gets the <see cref="IEntity"/> type that is held in the repository.
        /// </summary>
        Type EnclosedEntityType { get; }
    }

    /// <summary>
    /// Generic repository that holds an entity type.
    /// </summary>
    /// <typeparam name="T">The entity type that the repository holds.</typeparam>
    public interface IEntityRepository<T> : IReadOnlyEntityRepository<T>, IEntityRepository where T : IEntity
    {
        /// <summary>
        /// Adds an <see cref="IEntity"/> to the repository.
        /// </summary>
        /// <param name="entity">The <see cref="IEntity"/> to add.</param>
        void AddEntity(T entity);
    }
}