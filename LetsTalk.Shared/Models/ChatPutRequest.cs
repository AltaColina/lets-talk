using MediatR;
using System.Diagnostics.CodeAnalysis;

namespace LetsTalk.Models;

public sealed class ChatPutRequest : IRequest
{
    [NotNull] public Chat? Chat { get; init; }
}