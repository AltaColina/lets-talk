using LetsTalk.Rooms;
using MongoDB.Driver;

namespace LetsTalk.Repositories;

public interface IRoomRepository : IRepository<Room> { }

internal sealed class RoomRepository : MongoRepository<Room>, IRoomRepository
{
    public RoomRepository(IMongoDatabase database) : base(database)
    {
    }
}