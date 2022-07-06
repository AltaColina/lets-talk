using MediatR;
using System.Diagnostics.CodeAnalysis;

namespace LetsTalk.Models.Roles;

public sealed class RoleUserGetRequest : IRequest<RoleUserGetResponse>
{
    [NotNull] public string? RoleId { get; init; }
}
