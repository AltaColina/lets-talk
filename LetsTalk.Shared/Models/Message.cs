namespace LetsTalk.Models;

public sealed class Message
{
    public string ChatId { get; set; } = null!;
    public string Username { get; set; } = null!;
    public string Content { get; set; } = null!;
}