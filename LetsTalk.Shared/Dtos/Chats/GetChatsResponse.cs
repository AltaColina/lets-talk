namespace LetsTalk.Dtos.Chats;

public sealed class GetChatsResponse
{
    public List<ChatDto> Chats { get; init; } = null!;
}
