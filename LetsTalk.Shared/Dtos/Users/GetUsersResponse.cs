namespace LetsTalk.Dtos.Users;

public sealed class GetUsersResponse
{
    public List<UserDto> Users { get; init; } = new();
}