using MediatR;

namespace LetsTalk.Dtos.Roles;

public sealed class GetRoleUsersRequest : IRequest<GetRoleUsersResponse>
{
    public string Id { get; init; } = null!;
}
