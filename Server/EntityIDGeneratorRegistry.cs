using System;
using System.Collections.Generic;
using SharedClasses.Domain;

namespace Server
{
    /// <summary>
    /// Holds the <see cref="EntityIdGenerator" />s for each entity type that needs one.
    /// </summary>
    internal sealed class EntityIdGeneratorRegistry
    {
        /// <summary>
        /// A readonly version of an IDGenerator dictionary. No one can alter this dictionary after compiling.
        /// </summary>
        public readonly IReadOnlyDictionary<Type, EntityIdGenerator> EntityIDGeneratorsIndexedByEntityType =
            new Dictionary<Type, EntityIdGenerator>
            {
                { typeof(User), new EntityIdGenerator() },
                { typeof(Conversation), new EntityIdGenerator() },
                { typeof(IContribution), new EntityIdGenerator() },
                { typeof(Participation), new EntityIdGenerator() },
                { typeof(Avatar), new EntityIdGenerator() }
            };
    }
}