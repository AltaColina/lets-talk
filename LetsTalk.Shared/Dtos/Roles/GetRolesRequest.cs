using MediatR;

namespace LetsTalk.Dtos.Roles;

public sealed class GetRolesRequest : IRequest<GetRolesResponse>
{
    public string? Id { get; init; }
}
