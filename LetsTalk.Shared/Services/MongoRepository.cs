using Ardalis.Specification;
using LetsTalk.Interfaces;
using MongoDB.Bson;
using MongoDB.Driver;

namespace LetsTalk.Services;

public sealed class MongoRepository<T> : IRepository<T> where T : IEntity<string>
{
    private readonly IMongoCollection<T> _collection;

    public MongoRepository(IMongoDatabase database) => _collection = database.GetCollection<T>(typeof(T).Name);

    public async Task<IEnumerable<T>> GetAllAsync() =>
        (await _collection.FindAsync(new BsonDocument())).ToEnumerable();

    public async Task<IEnumerable<T>> GetAllAsync(ISpecification<T> specification) =>
        (await _collection.FindAsync(item => specification.IsSatisfiedBy(item))).ToEnumerable();

    public async Task<T?> GetAsync(string id) =>
        await _collection.Find(Builders<T>.Filter.Eq("_id", id)).SingleOrDefaultAsync();
    
    public async Task<T?> GetAsync(ISingleResultSpecification<T> specification) =>
        await (await _collection.FindAsync(item => specification.IsSatisfiedBy(item))).SingleOrDefaultAsync();

    public async Task InsertAsync(T entity) =>
        await _collection.InsertOneAsync(entity);
    
    public async Task UpdateAsync(T entity) =>
        await _collection.ReplaceOneAsync(Builders<T>.Filter.Eq("_id", entity.Id), entity);

    public async Task UpsertAsync(T entity) =>
        await _collection.ReplaceOneAsync(Builders<T>.Filter.Eq("_id", entity.Id), entity, new ReplaceOptions { IsUpsert = true });
    
    public async Task DeleteAsync(string id) =>
        await _collection.DeleteOneAsync(Builders<T>.Filter.Eq("_id", id));
}

