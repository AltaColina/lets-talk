namespace LetsTalk.Models;

public abstract class Message
{
    public string Id { get; init; } = Guid.NewGuid().ToString();
    public DateTimeOffset Timestamp { get; init; } = DateTimeOffset.UtcNow;
}