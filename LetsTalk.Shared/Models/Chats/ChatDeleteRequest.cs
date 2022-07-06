using MediatR;
using System.Diagnostics.CodeAnalysis;

namespace LetsTalk.Models.Chats;

public sealed class ChatDeleteRequest : IRequest
{
    [NotNull] public string? ChatId { get; init; }
}