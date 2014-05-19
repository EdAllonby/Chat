namespace Server
{
    /// <summary>
    /// Used to allocate a unique ID for an entity
    /// </summary>
    internal sealed class EntityIDGenerator
    {
        private int nextID;

        /// <summary>
        /// Gets a unique ID for an entity
        /// </summary>
        /// <returns>A unique ID</returns>
        public int GetEntityID()
        {
            return ++nextID;
        }
    }
}