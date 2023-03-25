using Duende.IdentityServer.Models;
using MongoDB.Driver;
using System.Linq.Expressions;

namespace LetsTalk.Repositories;
internal sealed class ClientRepository : MongoRepository<Client>, IClientRepository
{
    protected override Expression<Func<Client, string>> GetIdExpr { get; } = c => c.ClientId;
    protected override Expression<Func<Client, string>> GetNameExpr { get => c => c.ClientName; }

    public ClientRepository(IMongoDatabase database) : base(database) { }
}
