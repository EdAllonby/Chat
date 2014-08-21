using System;
using System.Collections.Generic;
using SharedClasses.Domain;

namespace SharedClasses
{
    public sealed class RepositoryManager : IService
    {
        private readonly IDictionary<Type, IRepository> repositoriesIndexedByEnclosedEntity = new Dictionary<Type, IRepository>();

        public void AddRepository<T>(IRepository repository) where T : IEntity
        {
            repositoriesIndexedByEnclosedEntity.Add(typeof (T), repository);
        }

        public IReadOnlyRepository<T> GetRepository<T>() where T : IEntity
        {
            IRepository repository;
            repositoriesIndexedByEnclosedEntity.TryGetValue(typeof (T), out repository);
            return (IReadOnlyRepository<T>) repository;
        }
    }
}