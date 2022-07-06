namespace LetsTalk.Models.Chats;

public sealed class ChatUserGetResponse
{
    public List<User> Users { get; init; } = new();
}
