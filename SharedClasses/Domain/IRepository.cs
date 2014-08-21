namespace SharedClasses.Domain
{
    /// <summary>
    /// Non-generic repository interface.
    /// </summary>
    public interface IRepository
    {
    }

    /// <summary>
    /// Generic repository that holds an entity type.
    /// </summary>
    /// <typeparam name="T">The entity type that the repository holds.</typeparam>
    public interface IRepository<T> : IReadOnlyRepository<T>, IRepository where T : IEntity
    {
        /// <summary>
        /// Adds an <see cref="IEntity"/> to the repository.
        /// </summary>
        /// <param name="entity">The <see cref="IEntity"/> to add.</param>
        void AddEntity(T entity);
    }
}