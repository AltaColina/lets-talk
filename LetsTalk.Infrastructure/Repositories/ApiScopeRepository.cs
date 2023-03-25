using Duende.IdentityServer.Models;
using MongoDB.Driver;

namespace LetsTalk.Repositories;

internal sealed class ApiScopeRepository : MongoResourceRepository<ApiScope>, IApiScopeRepository
{
    public ApiScopeRepository(IMongoDatabase database) : base(database) { }
}
