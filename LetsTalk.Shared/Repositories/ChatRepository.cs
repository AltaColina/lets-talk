using LetsTalk.Chats;
using MongoDB.Driver;

namespace LetsTalk.Repositories;

public interface IChatRepository : IRepository<Chat> { }

internal sealed class ChatRepository : MongoRepository<Chat>, IChatRepository
{
    public ChatRepository(IMongoDatabase database) : base(database)
    {
    }
}