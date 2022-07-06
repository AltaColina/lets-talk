using LiteDB;

namespace Microsoft.Extensions.DependencyInjection;

public sealed class LiteDbOptions
{
    public ConnectionString ConnectionString { get; set; } = new ConnectionString();
    public BsonMapper BsonMapper { get; set; } = BsonMapper.Global.UseCamelCase();
}
