using System.Collections.Generic;
using log4net;

namespace Server
{
    /// <summary>
    /// Holds the ID generators for entities
    /// </summary>
    public sealed class EntityGeneratorFactory
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (EntityGeneratorFactory));

        private readonly EntityIDGeneratorRegistry entityIDGeneratorRegistry = new EntityIDGeneratorRegistry();

        /// <summary>
        /// Gets the next available unique ID for the given entity.
        /// </summary>
        /// <typeparam name="T">The entity which wants a unique ID.</typeparam>
        /// <returns>A unique ID for the entity</returns>
        public int GetEntityID<T>()
        {
            EntityIDGenerator generator;
            bool isEntityGeneratorFound = entityIDGeneratorRegistry
                .EntityIDGeneratorsIndexedByEntityType.TryGetValue(typeof (T), out generator);

            if (!isEntityGeneratorFound)
            {
                Log.ErrorFormat("Entity generator for type {0} does not exist in {1}.", typeof (T), entityIDGeneratorRegistry);
                throw new KeyNotFoundException();
            }

            return generator.GetEntityID();
        }
    }
}