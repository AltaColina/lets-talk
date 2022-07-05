using LetsTalk.Interfaces;
using LetsTalk.Models;
using LiteDB;

namespace LetsTalk.Services;

public sealed class UserRepository : LiteDatabaseRepository<User>, IUserRepository
{
    public UserRepository(LiteDatabase liteDatabase) : base(liteDatabase)
    {
    }
}
