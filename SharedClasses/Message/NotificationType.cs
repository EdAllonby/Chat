namespace SharedClasses.Message
{
    /// <summary>
    /// Used to determine whether a notification <see cref="IMessage" /> is for creating, updating, or deleting an entity.
    /// </summary>
    public enum NotificationType
    {
        /// <summary>
        /// Signifies that the object should be created.
        /// </summary>
        Create,

        /// <summary>
        /// Signifies that the object has changed and needs updating.
        /// </summary>
        Update,

        /// <summary>
        /// Signifies that the object needs to be be deleted
        /// </summary>
        Delete
    }
}