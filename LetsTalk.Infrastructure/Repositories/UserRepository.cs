using LetsTalk.Users;
using MongoDB.Driver;

namespace LetsTalk.Repositories;

internal sealed class UserRepository : MongoRepository<User>, IUserRepository
{
    public UserRepository(IMongoDatabase database) : base(database)
    {
    }
}