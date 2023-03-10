using LetsTalk.Rooms;
using MongoDB.Driver;

namespace LetsTalk.Repositories;

internal sealed class RoomRepository : MongoRepository<Room>, IRoomRepository
{
    public RoomRepository(IMongoDatabase database) : base(database)
    {
    }
}