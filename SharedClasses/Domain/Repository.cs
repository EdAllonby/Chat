using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using log4net;
using SharedClasses.Message;

namespace SharedClasses.Domain
{
    public abstract class Repository<T> where T : IEntity
    {
        protected static readonly ILog Log = LogManager.GetLogger(typeof (Repository<T>));

        private readonly ConcurrentDictionary<int, T> entitiesIndexedById = new ConcurrentDictionary<int, T>();

        public event EventHandler<EntityChangedEventArgs<T>> EntityChanged;

        public event EventHandler<IEnumerable<T>> EntitiesAdded;

        /// <summary>
        /// Adds an <see cref="IEntity"/> to the repository.
        /// </summary>
        /// <param name="entity">The <see cref="IEntity"/> to add.</param>
        public void AddEntity(T entity)
        {
            Contract.Requires(!entity.Equals(null));

            entitiesIndexedById.TryAdd(entity.Id, entity);

            Log.DebugFormat("Entity with Id {0} added.", entity.Id);

            OnEntityAdded(entity);
        }

        /// <summary>
        /// TODO: I don't like this. This only exists for the sake of Participations. I might rethink how participations get added.
        /// </summary>
        /// <param name="entities"></param>
        public void AddEntities(IEnumerable<T> entities)
        {
            IEnumerable<T> entityEnumerable = entities as IList<T> ?? entities.ToList();

            foreach (T entity in entityEnumerable)
            {
                entitiesIndexedById.TryAdd(entity.Id, entity);
                Log.DebugFormat("Entity with Id {0} added.", entity.Id);

            }

            OnEntitiesAdded(entityEnumerable);
        }

        /// <summary>
        /// Retrieves an <see cref="IEntity"/> entity from the repository.
        /// </summary>
        /// <param name="entityId">The <see cref="IEntity"/> entity Id to find.</param>
        /// <returns>The <see cref="IEntity"/> which matches the ID. If no <see cref="IEntity"/> is found, return null.</returns>
        public T FindEntityById(int entityId)
        {
            return entitiesIndexedById[entityId];
        }

        private void OnEntitiesAdded(IEnumerable<T> entities)
        {
            EventHandler<IEnumerable<T>> entitiesAddedCopy = EntitiesAdded;

            if (entitiesAddedCopy != null)
            {
                entitiesAddedCopy(this, entities);
            }
        }

        /// <summary>
        /// Retrieves all <see cref="IEntity"/>s from the repository.
        /// </summary>
        /// <returns>A collection of all <see cref="IEntity"/>s in the repository.</returns>
        public IEnumerable<T> GetAllEntities()
        {
            return new List<T>(entitiesIndexedById.Values);
        }

        protected void OnEntityAdded(T entity)
        {
            EntityChangedEventArgs<T> entityChangedEventArgs = new EntityChangedEventArgs<T>(entity, NotificationType.Create);

            EventHandler<EntityChangedEventArgs<T>> entityChangedCopy = EntityChanged;

            if (entityChangedCopy != null)
            {
                entityChangedCopy(this, entityChangedEventArgs);
            }
        }

        protected void OnEntityUpdated(T entity, T previousEntity)
        {
            EntityChangedEventArgs<T> entityChangedEventArgs = new EntityChangedEventArgs<T>(entity, previousEntity);

            EventHandler<EntityChangedEventArgs<T>> entityChangedCopy = EntityChanged;

            if (entityChangedCopy != null)
            {
                entityChangedCopy(this, entityChangedEventArgs);
            }
        }
    }
}