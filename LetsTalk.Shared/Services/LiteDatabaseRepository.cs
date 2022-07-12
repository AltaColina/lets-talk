using LetsTalk.Interfaces;
using LiteDB;

namespace LetsTalk.Services;

public sealed class LiteDatabaseRepository<T> : IRepository<T>
{
    private ILiteCollection<T> _collection;

    public LiteDatabaseRepository(LiteDatabase liteDatabase) => _collection = liteDatabase.GetCollection<T>();

    public async Task<IEnumerable<T>> GetAllAsync() => await Task.FromResult(_collection.FindAll());
    public async Task<T?> GetAsync(string id) => await Task.FromResult(_collection.FindById(id));

    public async Task InsertAsync(T entity) => await Task.FromResult(_collection.Insert(entity));
    public async Task UpdateAsync(T entity) => await Task.FromResult(_collection.Update(entity));
    public async Task UpsertAsync(T entity) => await Task.FromResult(_collection.Upsert(entity));
    public async Task DeleteAsync(string id) => await Task.FromResult(_collection.Delete(id));
}


