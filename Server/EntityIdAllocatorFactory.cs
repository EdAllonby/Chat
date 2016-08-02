using System.Collections.Generic;
using log4net;
using SharedClasses;
using SharedClasses.Domain;

namespace Server
{
    /// <summary>
    /// Holds the Id generators for entities.
    /// </summary>
    public class EntityIdAllocatorFactory : IService
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(EntityIdAllocatorFactory));

        private readonly EntityIdGeneratorRegistry entityIdGeneratorRegistry = new EntityIdGeneratorRegistry();

        /// <summary>
        /// Gets the next available unique Id for the given entity.
        /// </summary>
        /// <typeparam name="T">The entity which wants a unique Id.</typeparam>
        /// <returns>A unique Id for the entity.</returns>
        public int AllocateEntityId<T>() where T : IEntity
        {
            EntityIdGenerator generator;

            bool isEntityGeneratorFound = entityIdGeneratorRegistry.EntityIDGeneratorsIndexedByEntityType.TryGetValue(typeof(T), out generator);

            if (!isEntityGeneratorFound)
            {
                Log.ErrorFormat($"Entity generator for type {typeof(T)} does not exist in {entityIdGeneratorRegistry}.");
                throw new KeyNotFoundException();
            }

            return generator.GenerateNextAvailableId();
        }
    }
}