using LetsTalk.Interfaces;
using LetsTalk.Models;
using MediatR;

namespace LetsTalk.Dtos.Roles;

public sealed class UpdateRoleRequest : IRequest, IMapTo<Role>
{
    public string Id { get; set; } = null!;
    public string Name { get; init; } = null!;
    public List<string> Permissions { get; set; } = new();
}
