using System.Collections.Generic;
using log4net;
using SharedClasses;

namespace Server
{
    /// <summary>
    /// Holds the Id generators for entities.
    /// </summary>
    public class EntityIdAllocatorFactory : IService
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (EntityIdAllocatorFactory));

        private readonly EntityIdGeneratorRegistry entityIdGeneratorRegistry = new EntityIdGeneratorRegistry();

        /// <summary>
        /// Gets the next available unique Id for the given entity.
        /// </summary>
        /// <typeparam name="T">The entity which wants a unique Id.</typeparam>
        /// <returns>A unique Id for the entity.</returns>
        public int AllocateEntityId<T>()
        {
            EntityIdGenerator generator;

            bool isEntityGeneratorFound = entityIdGeneratorRegistry
                .EntityIDGeneratorsIndexedByEntityType.TryGetValue(typeof (T), out generator);

            if (!isEntityGeneratorFound)
            {
                Log.ErrorFormat("Entity generator for type {0} does not exist in {1}.", typeof (T), entityIdGeneratorRegistry);
                throw new KeyNotFoundException();
            }

            return generator.GenerateNextAvailableId();
        }
    }
}