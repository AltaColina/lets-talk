using MongoDB.Driver;
using System.Linq.Expressions;

namespace LetsTalk.Repositories;

internal abstract class MongoEntityRepository<T> : MongoRepository<T> where T : Entity
{
    protected sealed override Expression<Func<T, string>> GetIdExpr { get; } = e => e.Id;
    protected sealed override Expression<Func<T, string>> GetNameExpr { get; } = e => e.Name;

    public MongoEntityRepository(IMongoDatabase database) : base(database) { }
}