using LetsTalk.Exceptions;
using LetsTalk.Interfaces;
using LetsTalk.Models;
using MediatR;

namespace LetsTalk.Commands;

public sealed class RoleUserGetRequestHandler : IRequestHandler<RoleUserGetRequest, RoleUserGetResponse>
{
    private readonly IRoleRepository _roleRepository;
    private readonly IUserRepository _userRepository;

    public RoleUserGetRequestHandler(IRoleRepository roleRepository, IUserRepository userRepository)
    {
        _roleRepository = roleRepository;
        _userRepository = userRepository;
    }

    public async Task<RoleUserGetResponse> Handle(RoleUserGetRequest request, CancellationToken cancellationToken)
    {
        var role = await _roleRepository.GetAsync(request.RoleId);
        if (role is null)
            throw new NotFoundException($"Role {request.RoleId} does not exist");
        var users = (await _userRepository.GetAllAsync()).Where(user => user.Roles.Contains(role.Id));
        return new RoleUserGetResponse { Users = new(users) };
    }
}
