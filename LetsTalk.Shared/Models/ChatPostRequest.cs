using MediatR;
using System.Diagnostics.CodeAnalysis;

namespace LetsTalk.Models;

public sealed class ChatPostRequest : IRequest
{
    [NotNull] public Chat? Chat { get; init; }
}
