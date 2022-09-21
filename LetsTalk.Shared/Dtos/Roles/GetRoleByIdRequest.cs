using MediatR;

namespace LetsTalk.Dtos.Roles;

public sealed class GetRoleByIdRequest : IRequest<RoleDto>
{
    public string Id { get; init; } = null!;
}
