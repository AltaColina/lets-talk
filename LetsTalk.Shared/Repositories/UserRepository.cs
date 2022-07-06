using LetsTalk.Interfaces;
using LetsTalk.Models;
using LiteDB;

namespace LetsTalk.Repositories;

public sealed class UserRepository : LiteDatabaseRepository<User>, IUserRepository
{
    public UserRepository(LiteDatabase liteDatabase) : base(liteDatabase)
    {
    }
}
