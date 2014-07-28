using SharedClasses.Domain;
using SharedClasses.Message;

namespace SharedClasses
{
    public sealed class EntityChangedEventArgs<T> where T : IEntity
    {
        public NotificationType NotificationType { get; private set; }

        public T Entity { get; private set; }

        public T PreviousEntity { get; private set; }

        public void EntityAdded(T entity)
        {
            NotificationType = NotificationType.Create;
            Entity = entity;
        }

        public void EntityUpdated(T entity, T previousEntity)
        {
            Entity = entity;
            PreviousEntity = previousEntity;
            NotificationType = NotificationType.Update;
        }

        public void EntityDeleted(T entity)
        {
            Entity = entity;
            NotificationType = NotificationType.Delete;
        }
    }
}