namespace LetsTalk.Interfaces;

public interface IRepository<T>
{
    Task<IEnumerable<T>> GetAllAsync();
    Task<T?> GetAsync(string id);
    Task InsertAsync(T entity);
    Task UpdateAsync(T entity);
    Task UpsertAsync(T entity);
    Task DeleteAsync(string id);
}
