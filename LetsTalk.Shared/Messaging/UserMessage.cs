namespace LetsTalk.Models;

public abstract class UserMessage : Message
{
    public string UserId { get; init; } = null!;
}