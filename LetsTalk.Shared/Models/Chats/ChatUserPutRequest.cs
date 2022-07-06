using MediatR;
using System.Diagnostics.CodeAnalysis;

namespace LetsTalk.Models.Chats;

public sealed class ChatUserPutRequest : IRequest
{
    [NotNull] public string? ChatId { get; init; }
    [NotNull] public string? UserId { get; init; }
}