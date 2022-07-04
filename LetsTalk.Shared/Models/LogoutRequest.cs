using MediatR;
using System.Diagnostics.CodeAnalysis;

namespace LetsTalk.Models;

public sealed class LogoutRequest : IRequest<Unit>
{
    [NotNull] public string? Username { get; init; }
}