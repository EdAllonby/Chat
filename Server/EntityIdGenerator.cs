using System.Threading;

namespace Server
{
    /// <summary>
    /// Used to create a unique Id number for an entity.
    /// </summary>
    internal sealed class EntityIdGenerator
    {
        private int nextId;

        /// <summary>
        /// Creates a thread-safe unique Id number for an entity.
        /// </summary>
        /// <returns>A unique Id number.</returns>
        public int GenerateNextAvailableId()
        {
            return Interlocked.Increment(ref nextId);
        }
    }
}