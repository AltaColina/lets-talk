namespace LetsTalk.Models.Chats;

public sealed class ChatGetResponse
{
    public List<Chat> Chats { get; init; } = new();
}