using Ardalis.Specification;

namespace LetsTalk.Interfaces;

public interface IRepository<T> : IRepositoryBase<T> where T : class, IEntity<string>
{
    string CollectionName { get; }
}
