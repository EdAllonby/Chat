using System;
using System.Collections.Generic;
using SharedClasses.Domain;

namespace Server
{
    /// <summary>
    /// Holds the <see cref="EntityIDGenerator"/>s for each entity type that needs one
    /// </summary>
    internal sealed class EntityIDGeneratorRegistry
    {
        private readonly EntityIDGenerator userIDGenerator = new EntityIDGenerator();
        private readonly EntityIDGenerator conversationIDGenerator = new EntityIDGenerator();
        private readonly EntityIDGenerator contributionIDGenerator = new EntityIDGenerator();

        private readonly Dictionary<Type, EntityIDGenerator> entityIDGeneratorsIndexedByEntityType = new Dictionary<Type, EntityIDGenerator>();

        public EntityIDGeneratorRegistry()
        {
            entityIDGeneratorsIndexedByEntityType.Add(typeof (User), userIDGenerator);           
            entityIDGeneratorsIndexedByEntityType.Add(typeof (Conversation), conversationIDGenerator);
            entityIDGeneratorsIndexedByEntityType.Add(typeof (Contribution), contributionIDGenerator);
        }

        public Dictionary<Type, EntityIDGenerator> EntityIDGeneratorsIndexedByEntityType
        {
            get { return entityIDGeneratorsIndexedByEntityType; }
        }
    }
}