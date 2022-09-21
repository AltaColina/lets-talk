using MediatR;

namespace LetsTalk.Dtos.Users;

public sealed class GetUserByIdRequest : IRequest<UserDto>
{
    public string Id { get; init; } = null!;
}
