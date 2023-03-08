using LetsTalk.Roles;
using MongoDB.Driver;

namespace LetsTalk.Repositories;

public interface IRoleRepository : IRepository<Role> { }

internal sealed class RoleRepository : MongoRepository<Role>, IRoleRepository
{
    public RoleRepository(IMongoDatabase database) : base(database)
    {
    }
}