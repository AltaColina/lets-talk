namespace LetsTalk.Models.Users;

public sealed class UserChatGetResponse
{
    public List<Chat> Chats { get; init; } = new();
}
