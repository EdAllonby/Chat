using System;
using System.Collections.Generic;
using SharedClasses.Domain;

namespace SharedClasses
{
    /// <summary>
    /// Holds references to repositories.
    /// </summary>
    public sealed class RepositoryManager : IService
    {
        private readonly IDictionary<Type, IEntityRepository> repositoriesIndexedByEnclosedEntity = new Dictionary<Type, IEntityRepository>();

        /// <summary>
        /// Add a repository to the <see cref="RepositoryManager" />.
        /// </summary>
        /// <typeparam name="T">The type that the repository holds.</typeparam>
        /// <param name="repository">The repository instance to add.</param>
        public void AddRepository<T>(IEntityRepository repository) where T : IEntity
        {
            repositoriesIndexedByEnclosedEntity.Add(typeof(T), repository);
        }

        /// <summary>
        /// Get a repository from the <see cref="RepositoryManager" />.
        /// </summary>
        /// <typeparam name="T">The type of <see cref="IEntity" /> that the repository holds.</typeparam>
        /// <returns>A readonly version of the repository requested. If no repository is found, return null.</returns>
        public IReadOnlyEntityRepository<T> GetRepository<T>() where T : IEntity
        {
            IEntityRepository repository;
            repositoriesIndexedByEnclosedEntity.TryGetValue(typeof(T), out repository);
            return (IReadOnlyEntityRepository<T>) repository;
        }
    }
}