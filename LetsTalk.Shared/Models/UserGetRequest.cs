using MediatR;

namespace LetsTalk.Models;

public sealed class UserGetRequest : IRequest<UserGetResponse>
{
    public string? UserId { get; init; }
}
