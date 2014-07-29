using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using log4net;

namespace SharedClasses.Domain
{
    public abstract class Repository<T> where T : IEntity
    {
        protected static readonly ILog Log = LogManager.GetLogger(typeof(Repository<T>));

        protected readonly ConcurrentDictionary<int, T> EntitiesIndexedById = new ConcurrentDictionary<int, T>();

        public event EventHandler<EntityChangedEventArgs<T>> EntityChanged;

        /// <summary>
        /// Adds an <see cref="IEntity"/> to the repository.
        /// </summary>
        /// <param name="entity">The <see cref="IEntity"/> to add.</param>
        public void AddEntity(T entity)
        {
            Contract.Requires(!entity.Equals(null));

            EntitiesIndexedById.TryAdd(entity.Id, entity);
            Log.DebugFormat("User with Id {0} added.", entity.Id);

            var userChangedEventArgs = new EntityChangedEventArgs<T>();

            userChangedEventArgs.EntityAdded(entity);

            OnEntityChanged(userChangedEventArgs);
        }

        /// <summary>
        /// Retrieves an <see cref="IEntity"/> entity from the repository.
        /// </summary>
        /// <param name="userId">The <see cref="IEntity"/> entity Id to find.</param>
        /// <returns>The <see cref="IEntity"/> which matches the ID. If no <see cref="IEntity"/> is found, return null.</returns>
        public T FindEntityById(int userId)
        {
            return EntitiesIndexedById[userId];
        }

        protected void OnEntityChanged(EntityChangedEventArgs<T> entityChangedEventArgs)
        {
            EventHandler<EntityChangedEventArgs<T>> userChangedCopy = EntityChanged;

            if (userChangedCopy != null)
            {
                userChangedCopy(this, entityChangedEventArgs);
            }
        }

        /// <summary>
        /// Retrieves all <see cref="IEntity"/>s from the repository.
        /// </summary>
        /// <returns>A collection of all <see cref="IEntity"/>s in the repository.</returns>
        public IEnumerable<T> GetAllEntities()
        {
            return EntitiesIndexedById.Values;
        }
    }
}