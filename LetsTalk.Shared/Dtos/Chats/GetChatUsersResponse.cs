using LetsTalk.Dtos.Users;

namespace LetsTalk.Dtos.Chats;

public sealed class GetChatUsersResponse
{
    public List<UserDto> Users { get; init; } = null!;
}
