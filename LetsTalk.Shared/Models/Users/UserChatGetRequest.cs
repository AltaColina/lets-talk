using MediatR;
using System.Diagnostics.CodeAnalysis;

namespace LetsTalk.Models.Users;

public sealed class UserChatGetRequest : IRequest<UserChatGetResponse>
{
    [NotNull] public string? UserId { get; init; }
}
