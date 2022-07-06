using LetsTalk.Exceptions;
using LetsTalk.Interfaces;
using LetsTalk.Models.Users;
using MediatR;

namespace LetsTalk.Commands.Users;

public sealed class UserRoleGetRequestHandler : IRequestHandler<UserRoleGetRequest, UserRoleGetResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;

    public UserRoleGetRequestHandler(IUserRepository userRepository, IRoleRepository roleRepository)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
    }

    public async Task<UserRoleGetResponse> Handle(UserRoleGetRequest request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetAsync(request.UserId);
        if (user is null)
            throw new NotFoundException($"User {request.UserId} does not exist");
        var roles = (await _roleRepository.GetAllAsync()).Where(role => user.Roles.Contains(role.Id));
        return new UserRoleGetResponse { Roles = new(roles) };
    }
}
