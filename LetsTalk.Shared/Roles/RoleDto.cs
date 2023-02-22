using LetsTalk.Interfaces;

namespace LetsTalk.Roles;

public sealed class RoleDto : IMapFrom<Role>
{
    public string Id { get; set; } = null!;
    public string Name { get; init; } = null!;
    public List<string> Permissions { get; set; } = new();
}
