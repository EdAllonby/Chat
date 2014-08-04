using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using log4net;

namespace SharedClasses.Domain
{
    public abstract class Repository<T> where T : IEntity
    {
        protected static readonly ILog Log = LogManager.GetLogger(typeof (Repository<T>));

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
            Log.DebugFormat("Entity with Id {0} added.", entity.Id);

            var entityChangedEventArgs = new EntityChangedEventArgs<T>();

            entityChangedEventArgs.EntityAdded(entity);

            OnEntityChanged(entityChangedEventArgs);
        }

        /// <summary>
        /// Retrieves an <see cref="IEntity"/> entity from the repository.
        /// </summary>
        /// <param name="entityId">The <see cref="IEntity"/> entity Id to find.</param>
        /// <returns>The <see cref="IEntity"/> which matches the ID. If no <see cref="IEntity"/> is found, return null.</returns>
        public T FindEntityById(int entityId)
        {
            return EntitiesIndexedById[entityId];
        }

        protected void OnEntityChanged(EntityChangedEventArgs<T> entityChangedEventArgs)
        {
            EventHandler<EntityChangedEventArgs<T>> entityChangedCopy = EntityChanged;

            if (entityChangedCopy != null)
            {
                entityChangedCopy(this, entityChangedEventArgs);
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