using System.Collections.Generic;
using log4net;

namespace Server
{
    /// <summary>
    /// Holds the ID generators for entities
    /// </summary>
    public static class EntityGeneratorFactory
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (EntityGeneratorFactory));

        private static readonly EntityIDGeneratorRegistry EntityIDGeneratorRegistry = new EntityIDGeneratorRegistry();

        /// <summary>
        /// Gets the next available unique ID for the given entity.
        /// </summary>
        /// <typeparam name="T">The entity which wants a unique ID.</typeparam>
        /// <returns>A unique ID for the entity</returns>
        public static int GetEntityID<T>()
        {
            EntityIDGenerator generator;
            bool isEntityGeneratorFound = EntityIDGeneratorRegistry
                .EntityIDGeneratorsIndexedByEntityType.TryGetValue(typeof (T), out generator);

            if (!isEntityGeneratorFound)
            {
                Log.ErrorFormat("Entity generator for type {0} does not exist in {1}.", typeof (T), EntityIDGeneratorRegistry);
                throw new KeyNotFoundException();
            }

            return generator.GetEntityID();
        }
    }
}