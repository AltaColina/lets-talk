using MediatR;

namespace LetsTalk.Models.Roles;

public sealed class RoleGetRequest : IRequest<RoleGetResponse>
{
    public string? RoleId { get; init; }
}
