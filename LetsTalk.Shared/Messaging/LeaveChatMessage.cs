using LetsTalk.Dtos;

namespace LetsTalk.Messaging;

public sealed class LeaveChatMessage : Message<UserDto>
{
    public ChatDto Chat { get; init; } = null!;
}
