using LetsTalk.Exceptions;
using LetsTalk.Interfaces;
using LetsTalk.Models.Users;
using MediatR;

namespace LetsTalk.Commands.Users;

public sealed class UserRolePutRequestHandler : IRequestHandler<UserRolePutRequest>
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;

    public UserRolePutRequestHandler(IUserRepository userRepository, IRoleRepository roleRepository)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
    }

    public async Task<Unit> Handle(UserRolePutRequest request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetAsync(request.UserId);
        if (user is null)
            throw new NotFoundException($"User {request.UserId} does not exist");
        // Get only valid roles.
        var roles = (await _roleRepository.GetAllAsync())
            .Select(role => role.Id)
            .Intersect(request.Roles);
        user.Roles.UnionWith(roles);
        await _userRepository.UpdateAsync(user);
        return Unit.Value;
    }
}