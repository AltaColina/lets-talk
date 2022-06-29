using LetsTalk.Interfaces;
using LetsTalk.Models;
using LiteDB;

namespace LetsTalk.Services;

public class UserRepository : LiteDatabaseRepository<User>, IUserRepository
{
    public UserRepository(LiteDatabase liteDatabase) : base(liteDatabase) { }
}
