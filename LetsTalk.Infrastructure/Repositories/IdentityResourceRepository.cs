using Duende.IdentityServer.Models;
using MongoDB.Driver;

namespace LetsTalk.Repositories;

internal sealed class IdentityResourceRepository : MongoResourceRepository<IdentityResource>, IIdentityResourceRepository
{
    public IdentityResourceRepository(IMongoDatabase database) : base(database) { }
}