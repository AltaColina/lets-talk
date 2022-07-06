using MediatR;
using System.Diagnostics.CodeAnalysis;

namespace LetsTalk.Models.Chats;

public sealed class ChatUserGetRequest : IRequest<ChatUserGetResponse>
{
    [NotNull] public string? ChatId { get; init; }
}
