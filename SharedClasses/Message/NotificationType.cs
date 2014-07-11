namespace SharedClasses.Message
{
    /// <summary>
    /// Used to determine whether a notification <see cref="IMessage"/> is for creating, updating, or deleting an entity.
    /// </summary>
    public enum NotificationType
    {
        Create,
        Update,
        Delete,
    }
}