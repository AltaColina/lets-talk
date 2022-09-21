using MediatR;

namespace LetsTalk.Dtos.Users;

public sealed class GetUsersRequest : IRequest<GetUsersResponse>
{
    public string? Id { get; init; }
}
