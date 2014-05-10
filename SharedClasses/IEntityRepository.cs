using System.Collections.Generic;

namespace SharedClasses.Domain
{
    public interface IEntityRepository
    {
        void AddEntity(object entity);

        void AddEntities(IEnumerable<object> entities);
    }

    public interface IEntityRepository<T> : IEntityRepository
    {
        void AddEntity(T entity);

        void AddEntities(IEnumerable<T> entities);

        void RemoveEntity(int entityID);

        T FindEntityByID(int entityID);

        IEnumerable<T> GetAllEntities();
    }
}