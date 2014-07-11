namespace Server
{
    /// <summary>
    /// Used to create a unique Id number for an entity.
    /// </summary>
    internal sealed class EntityIdGenerator
    {
        private int nextId;

        /// <summary>
        /// Creates a unique Id number for an entity.
        /// </summary>
        /// <returns>A unique Id number</returns>
        public int GenerateNextAvailableId()
        {
            return ++nextId;
        }
    }
}