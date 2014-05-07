namespace Server
{
    public sealed class EntityIDGenerator
    {
        private int nextId;

        public int AssignEntityId()
        {
            return nextId++;
        }
    }
}