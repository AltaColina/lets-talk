using LetsTalk.Chats;
using LetsTalk.Users;

namespace LetsTalk.Messaging;

public sealed class JoinChatMessage : Message<UserDto>
{
    public ChatDto Chat { get; init; } = null!;
}