using LetsTalk.Interfaces;
using LiteDB;

namespace LetsTalk.Services;

public abstract class LiteDatabaseRepository<T> : IRepository<T>
{
    protected ILiteCollection<T> Collection { get; }

    public LiteDatabaseRepository(LiteDatabase liteDatabase) => Collection = liteDatabase.GetCollection<T>();

    public async Task<IEnumerable<T>> GetAllAsync() => await Task.FromResult(Collection.FindAll());
    public async Task<T?> GetAsync(string id) => await Task.FromResult(Collection.FindById(id));

    public async Task InsertAsync(T entity) => await Task.FromResult(Collection.Insert(entity));
    public async Task UpdateAsync(T entity) => await Task.FromResult(Collection.Update(entity));
    public async Task UpsertAsync(T entity) => await Task.FromResult(Collection.Upsert(entity));
    public async Task DeleteAsync(string id) => await Task.FromResult(Collection.Delete(id));
}
