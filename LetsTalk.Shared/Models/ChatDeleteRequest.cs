using MediatR;
using System.Diagnostics.CodeAnalysis;

namespace LetsTalk.Models;

public sealed class ChatDeleteRequest : IRequest
{
    [NotNull] public string? ChatId { get; init; }
}