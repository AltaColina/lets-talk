using LetsTalk.Interfaces;
using LetsTalk.Models;
using LiteDB;
using System.Collections.Concurrent;

namespace LetsTalk.Services;

public sealed class ChatRepository : LiteDatabaseRepository<Chat>, IChatRepository
{
    public ChatRepository(LiteDatabase liteDatabase) : base(liteDatabase)
    {
        Collection.Insert(new Chat
        {
            Id = Guid.NewGuid().ToString(),
            Name = "General"
        });
    }
}
