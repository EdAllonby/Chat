using System;
using System.Collections.Generic;
using SharedClasses.Domain;

namespace Server
{
    /// <summary>
    /// Holds the <see cref="EntityIDGenerator"/>s for each entity type that needs one.
    /// </summary>
    internal sealed class EntityIDGeneratorRegistry
    {
        /// <summary>
        /// A readonly version of an IDGenerator dictionary. No one can alter this dictionary after compiling.
        /// </summary>
        public readonly IReadOnlyDictionary<Type, EntityIDGenerator> EntityIDGeneratorsIndexedByEntityType =
            new Dictionary<Type, EntityIDGenerator>
            {
                {typeof (User), new EntityIDGenerator()},
                {typeof (Conversation), new EntityIDGenerator()},
                {typeof (Contribution), new EntityIDGenerator()}
            };
    }
}