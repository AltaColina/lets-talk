using LetsTalk.Interfaces;

namespace LetsTalk.Users;
public sealed class UserDto : IMapFrom<User>
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public string? ImageUrl { get; init; }
    public required DateTimeOffset LastLoginTime { get; init; }
    public required HashSet<string> Roles { get; init; }
    public required HashSet<string> Rooms { get; init; }
}
