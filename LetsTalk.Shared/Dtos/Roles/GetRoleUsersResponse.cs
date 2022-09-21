using LetsTalk.Dtos.Users;

namespace LetsTalk.Dtos.Roles;

public sealed class GetRoleUsersResponse
{
    public List<UserDto> Users { get; init; } = new();
}
