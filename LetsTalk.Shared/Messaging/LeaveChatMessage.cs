using LetsTalk.Dtos;
using LetsTalk.Messaging.Abstract;

namespace LetsTalk.Models;

public sealed class LeaveChatMessage : Message<UserDto>
{
    public ChatDto Chat { get; init; } = null!;
}
