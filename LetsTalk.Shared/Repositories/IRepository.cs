using Ardalis.Specification;

namespace LetsTalk.Repositories;

public interface IRepository<T> : IRepositoryBase<T> where T : class, IEntity<string>
{
    string CollectionName { get; }
}
