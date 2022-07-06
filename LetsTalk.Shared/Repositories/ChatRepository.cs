using LetsTalk.Interfaces;
using LetsTalk.Models;
using LiteDB;

namespace LetsTalk.Repositories;

public sealed class ChatRepository : LiteDatabaseRepository<Chat>, IChatRepository
{
    public ChatRepository(LiteDatabase liteDatabase) : base(liteDatabase)
    {
    }
}
