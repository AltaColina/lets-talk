using MediatR;
using System.Diagnostics.CodeAnalysis;

namespace LetsTalk.Models;

public sealed class RoleGetRequest : IRequest<RoleGetResponse>
{
    [NotNull] public string? UserId { get; init; }
}
