using System.Collections.Generic;

namespace SharedClasses
{
    /// <summary>
    /// All repositories will implement this generic interface for basic CRUD operations
    /// </summary>
    /// <typeparam name="T">The entity type of the repository</typeparam>
    public interface IEntityRepository<T>
    {
        /// <summary>
        /// Adds the entity to the repository
        /// </summary>
        /// <param name="entity">Entity to be added to the repository</param>
        void AddEntity(T entity);

        /// <summary>
        /// Adds multiple entities to the repository
        /// </summary>
        /// <param name="entities">Entities to be added to the repository</param>
        void AddEntities(IEnumerable<T> entities);

        /// <summary>
        /// Removes an entity from the repository by its ID
        /// </summary>
        /// <param name="entityID">The entity's ID to find the entity to be removed from the repository</param>
        void RemoveEntity(int entityID);

        /// <summary>
        /// Finds the entity in the repository by its ID
        /// </summary>
        /// <param name="entityID">The entity's ID to find the entity to be returned </param>
        /// <returns>The entity requested by its ID</returns>
        T FindEntityByID(int entityID);

        /// <summary>
        /// Returns all entities currently in the repository
        /// </summary>
        /// <returns>The entities in the repository</returns>
        IEnumerable<T> GetAllEntities();
    }
}