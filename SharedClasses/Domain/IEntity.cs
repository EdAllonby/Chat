namespace SharedClasses.Domain
{
    /// <summary>
    /// Defines something as a domain entity in our Chat Program world.
    /// </summary>
    public interface IEntity
    {
        /// <summary>
        /// The unique Id of this entity.
        /// </summary>
        int Id { get; }
    }
}