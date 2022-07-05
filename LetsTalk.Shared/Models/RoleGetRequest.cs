using MediatR;

namespace LetsTalk.Models;

public sealed class RoleGetRequest : IRequest<RoleGetResponse>
{
    public string? RoleId { get; init; }
}
