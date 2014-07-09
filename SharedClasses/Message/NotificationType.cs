namespace SharedClasses.Message
{
    /// <summary>
    /// Used to determine whether a notification <see cref="IMessage"/> is for creation of deletion.
    /// </summary>
    public enum NotificationType
    {
        Create,
        Update,
        Delete,
    }
}