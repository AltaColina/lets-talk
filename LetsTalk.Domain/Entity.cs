using System.Diagnostics.CodeAnalysis;

namespace LetsTalk;
public abstract class Entity
{
    public string Id { get; init; }
    public required string Name { get; set; }
    public DateTimeOffset CreationTime { get; init; }
    public DateTimeOffset LastModified { get; init; }

    protected Entity()
    {
        Id = Guid.NewGuid().ToString();
        CreationTime = LastModified = DateTimeOffset.UtcNow;
    }

    [SetsRequiredMembers]
    protected Entity(Entity other)
    {
        if (other is null)
            throw new ArgumentNullException(nameof(other));

        Id = other.Id;
        Name = other.Name;
        CreationTime = other.CreationTime;
        LastModified = other.LastModified;
    }
}