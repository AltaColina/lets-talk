using LetsTalk.Interfaces;

namespace LetsTalk.Chats;

public sealed class ChatDto : IMapFrom<Chat>
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
}