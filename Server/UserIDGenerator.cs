namespace Server
{
    public sealed class UserIDGenerator
    {
        private int nextId;

        public int CreateUserId()
        {
            return nextId++;
        }
    }
}