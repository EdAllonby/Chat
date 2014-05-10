using SharedClasses.Domain;

namespace SharedClasses
{
    /// <summary>
    /// returns the correct repository for an entity
    /// </summary>
    public sealed class RepositoryFactory
    {
        private readonly IEntityRepository<Conversation> conversationRepository = new ConversationRepository();
        private readonly IEntityRepository<User> userRepository = new UserRepository();

        /// <summary>
        /// Returns the correct <see cref="IEntityRepository"/> based on the type of the generic type T
        /// </summary>
        /// <typeparam name="T">The entity</typeparam>
        /// <returns>The type of repository to return based on the entity</returns>
        public IEntityRepository<T> GetRepository<T>()
        {
            if (typeof (T) == typeof (User))
            {
                return userRepository as IEntityRepository<T>;
            }
            if (typeof (T) == typeof (Conversation))
            {
                return conversationRepository as IEntityRepository<T>;
            }

            return null;
        }
    }
}