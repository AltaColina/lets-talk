namespace LetsTalk.Dtos.Roles;

public sealed class GetRolesResponse
{
    public List<RoleDto> Roles { get; init; } = new();
}