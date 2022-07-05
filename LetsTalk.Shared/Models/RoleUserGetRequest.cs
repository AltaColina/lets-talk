using MediatR;
using System.Diagnostics.CodeAnalysis;

namespace LetsTalk.Models;

public sealed class RoleUserGetRequest : IRequest<RoleUserGetResponse>
{
    [NotNull] public string? RoleId { get; init; }
}
