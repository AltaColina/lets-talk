using MediatR;
using System.Diagnostics.CodeAnalysis;

namespace LetsTalk.Models;

public sealed class RoleGetRequest : IRequest<RoleGetResponse>
{
    public string? RoleId { get; init; }
}
