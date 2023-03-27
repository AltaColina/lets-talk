using Duende.IdentityServer.Models;
using MongoDB.Driver;

namespace LetsTalk.Repositories;

internal sealed class ApiResourceRepository : MongoResourceRepository<ApiResource>, IApiResourceRepository
{
    public ApiResourceRepository(IMongoDatabase database) : base(database) { }
}
