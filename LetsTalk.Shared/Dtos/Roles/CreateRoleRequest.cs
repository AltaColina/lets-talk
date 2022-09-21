using LetsTalk.Interfaces;
using LetsTalk.Models;
using MediatR;

namespace LetsTalk.Dtos.Roles;

public sealed class CreateRoleRequest : IRequest<RoleDto>, IMapTo<Role>
{
    public string Id { get; init; } = null!;
    public string Name { get; init; } = null!;
    public List<string> Permissions { get; set; } = new();
}