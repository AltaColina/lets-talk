namespace LetsTalk.Messaging;

public abstract class Message<T> : Message
{
    public T Content { get; init; } = default!;
}

public abstract class Message
{
    public string Id { get; init; } = Guid.NewGuid().ToString();
    public DateTimeOffset Timestamp { get; init; } = DateTimeOffset.UtcNow;
}