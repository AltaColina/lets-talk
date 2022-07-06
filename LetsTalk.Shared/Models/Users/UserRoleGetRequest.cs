using MediatR;
using System.Diagnostics.CodeAnalysis;

namespace LetsTalk.Models.Users;

public sealed class UserRoleGetRequest : IRequest<UserRoleGetResponse>
{
    [NotNull] public string? UserId { get; init; }
}
