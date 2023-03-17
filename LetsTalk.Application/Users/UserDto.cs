using LetsTalk.Interfaces;

namespace LetsTalk.Users;
public sealed class UserDto : IMapFrom<User>
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public string? ImageUrl { get; init; }
}
