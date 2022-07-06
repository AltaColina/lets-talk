using LetsTalk.Interfaces;
using LetsTalk.Models;
using LiteDB;

namespace LetsTalk.Repositories;

public sealed class RoleRepository : LiteDatabaseRepository<Role>, IRoleRepository
{
    public RoleRepository(LiteDatabase liteDatabase) : base(liteDatabase)
    {
    }
}
