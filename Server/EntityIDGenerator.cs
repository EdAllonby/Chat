namespace Server
{
    public sealed class EntityIDGenerator
    {
        private int nextID;

        public int AssignEntityID()
        {
            return ++nextID;
        }
    }
}