using LetsTalk.Interfaces;
using LetsTalk.Models;
using LiteDB;

namespace LetsTalk.Services;

public sealed class RoleRepository : LiteDatabaseRepository<Role>, IRoleRepository
{
    public RoleRepository(LiteDatabase liteDatabase) : base(liteDatabase)
    {
    }
}
