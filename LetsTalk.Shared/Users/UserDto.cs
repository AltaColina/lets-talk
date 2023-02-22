using LetsTalk.Interfaces;

namespace LetsTalk.Users;
public sealed class UserDto : IMapFrom<User>
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public HashSet<string> Roles { get; set; } = new();
    public HashSet<string> Chats { get; set; } = new();
    public DateTimeOffset CreationTime { get; set; }
    public DateTimeOffset LastLoginTime { get; set; }
}
