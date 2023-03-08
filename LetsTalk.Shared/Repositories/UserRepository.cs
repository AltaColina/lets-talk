using LetsTalk.Users;
using MongoDB.Driver;

namespace LetsTalk.Repositories;

public interface IUserRepository : IRepository<User> { }

internal sealed class UserRepository : MongoRepository<User>, IUserRepository
{
    public UserRepository(IMongoDatabase database) : base(database)
    {
    }
}