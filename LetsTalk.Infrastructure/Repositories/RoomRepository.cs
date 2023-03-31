using LetsTalk.Rooms;
using MongoDB.Driver;

namespace LetsTalk.Repositories;

internal sealed class RoomRepository : MongoEntityRepository<Room>, IRoomRepository
{
    public RoomRepository(IMongoDatabase database) : base(database)
    {
    }
}