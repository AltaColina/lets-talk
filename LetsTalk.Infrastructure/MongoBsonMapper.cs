using Duende.IdentityServer.Models;
using MongoDB.Bson.Serialization;

namespace LetsTalk;
internal static class MongoBsonMapper
{
    public static void RegisterClassMaps()
    {
        BsonClassMap.RegisterClassMap<Resource>(m =>
        {
            m.AutoMap();
            m.MapIdMember(r => r.Name);
        });

        BsonClassMap.RegisterClassMap<Client>(m =>
        {
            m.AutoMap();
            m.MapIdMember(r => r.ClientId);
        });
    }
}
