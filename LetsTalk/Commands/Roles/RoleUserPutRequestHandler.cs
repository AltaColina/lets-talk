using LetsTalk.Exceptions;
using LetsTalk.Interfaces;
using LetsTalk.Models.Roles;
using MediatR;

namespace LetsTalk.Commands.Roles;

public sealed class RoleUserPutRequestHandler : IRequestHandler<RoleUserPutRequest>
{
    private readonly IRoleRepository _roleRepository;
    private readonly IUserRepository _userRepository;

    public RoleUserPutRequestHandler(IRoleRepository roleRepository, IUserRepository userRepository)
    {
        _roleRepository = roleRepository;
        _userRepository = userRepository;
    }

    public async Task<Unit> Handle(RoleUserPutRequest request, CancellationToken cancellationToken)
    {
        var role = await _roleRepository.GetAsync(request.RoleId);
        if (role is null)
            throw new NotFoundException($"Role {request.RoleId} does not exist");
        var user = await _userRepository.GetAsync(request.UserId);
        if (user is null)
            throw new NotFoundException($"User {request.UserId} does not exist");
        user.Roles.Add(role.Id);
        await _userRepository.UpdateAsync(user);
        return Unit.Value;
    }
}
