using MediatR;
using System.Diagnostics.CodeAnalysis;

namespace LetsTalk.Models;

public sealed class ChatPostRequest : IRequest<ChatPostResponse>
{
    [NotNull] public string? Name { get; init; }
}
