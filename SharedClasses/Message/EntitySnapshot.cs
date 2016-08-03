using System;
using System.Collections.Generic;
using SharedClasses.Domain;

namespace SharedClasses.Message
{
    /// <summary>
    /// Packages a collection of <see cref="IEntity" />s for the client to update their repository at startup.
    /// </summary>
    /// <typeparam name="TEntity">The snapshot <see cref="IEntity" />. </typeparam>
    [Serializable]
    public sealed class EntitySnapshot<TEntity> : IMessage where TEntity : IEntity
    {
        private readonly SnapshotMessageIdentifierFactory snapshotMessageIdentifierFactory = new SnapshotMessageIdentifierFactory();

        /// <summary>
        /// Create a snapshot containing <see cref="IEntity" />s of type <see cref="TEntity" />.
        /// </summary>
        /// <param name="entities">the entities to packages as a snapshot.</param>
        public EntitySnapshot(IEnumerable<TEntity> entities)
        {
            Entities = entities;
        }

        /// <summary>
        /// The entities packages as a Snapshot.
        /// </summary>
        public IEnumerable<TEntity> Entities { get; private set; }

        /// <summary>
        /// The type of Snapshot.
        /// </summary>
        public MessageIdentifier MessageIdentifier => snapshotMessageIdentifierFactory.GetIdentifierBySnapshotType(typeof(TEntity));
    }
}