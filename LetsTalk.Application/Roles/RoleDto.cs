using LetsTalk.Interfaces;

namespace LetsTalk.Roles;

public sealed class RoleDto : IMapFrom<Role>
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public required List<string> Permissions { get; init; }
}
