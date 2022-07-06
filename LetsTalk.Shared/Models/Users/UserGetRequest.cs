using MediatR;

namespace LetsTalk.Models.Users;

public sealed class UserGetRequest : IRequest<UserGetResponse>
{
    public string? UserId { get; init; }
}
