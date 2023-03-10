using LetsTalk.Roles;
using MongoDB.Driver;

namespace LetsTalk.Repositories;

internal sealed class RoleRepository : MongoRepository<Role>, IRoleRepository
{
    public RoleRepository(IMongoDatabase database) : base(database)
    {
    }
}