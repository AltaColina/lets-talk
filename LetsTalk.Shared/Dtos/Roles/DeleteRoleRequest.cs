using MediatR;

namespace LetsTalk.Dtos.Roles;

public sealed class DeleteRoleRequest : IRequest
{
    public string Id { get; init; } = null!;
}