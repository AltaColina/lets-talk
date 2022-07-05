using LetsTalk.Exceptions;
using LetsTalk.Interfaces;
using LetsTalk.Models;
using MediatR;

namespace LetsTalk.Commands;

public sealed class UserRoleGetRequestHandler : IRequestHandler<UserRoleGetRequest, UserRoleGetResponse>
{
    private readonly IUserRepository _userRepository;

    public UserRoleGetRequestHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserRoleGetResponse> Handle(UserRoleGetRequest request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetAsync(request.UserId);
        if (user is null)
            throw new NotFoundException($"User {request.UserId} does not exist");
        return new UserRoleGetResponse { Roles = new(user.Roles) };
    }
}
