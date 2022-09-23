using LetsTalk.Dtos;
using LetsTalk.Messaging.Abstract;

namespace LetsTalk.Messaging;

public sealed class JoinChatMessage : Message<UserDto>
{
    public ChatDto Chat { get; init; } = null!;
}