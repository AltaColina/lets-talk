namespace LetsTalk.Models;

public sealed class ChatGetResponse
{
    public List<Chat> Chats { get; init; } = new();
}