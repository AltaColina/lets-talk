namespace LetsTalk.Models;

public sealed class ChatMessage : UserMessage
{
    public string ChatId { get; set; } = null!;
    public string Content { get; set; } = null!;
}
