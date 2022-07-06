using MediatR;
using System.Diagnostics.CodeAnalysis;

namespace LetsTalk.Models.Users;

public sealed class UserChatPutRequest : IRequest
{
    [NotNull] public string? UserId { get; init; }
    public List<string> Chats { get; init; } = new();
}