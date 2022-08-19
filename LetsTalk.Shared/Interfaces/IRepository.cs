using Ardalis.Specification;

namespace LetsTalk.Interfaces;

public interface IRepository<T> where T : IEntity<string>
{
    Task<IEnumerable<T>> GetAllAsync();
    Task<IEnumerable<T>> GetAllAsync(ISpecification<T> specification);
    Task<T?> GetAsync(string id);
    Task<T?> GetAsync(ISingleResultSpecification<T> specification);
    Task InsertAsync(T entity);
    Task UpdateAsync(T entity);
    Task UpsertAsync(T entity);
    Task DeleteAsync(string id);
}
