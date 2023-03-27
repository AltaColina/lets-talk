using Duende.IdentityServer.Models;
using MongoDB.Driver;
using System.Linq.Expressions;

namespace LetsTalk.Repositories;

internal abstract class MongoResourceRepository<T> : MongoRepository<T> where T : Resource
{
    protected sealed override Expression<Func<T, string>> GetIdExpr { get; } = e => e.Name;
    protected sealed override Expression<Func<T, string>> GetNameExpr { get => GetIdExpr; }

    public MongoResourceRepository(IMongoDatabase database) : base(database) { }
}
