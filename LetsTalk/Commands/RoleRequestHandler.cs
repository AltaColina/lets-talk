using LetsTalk.Exceptions;
using LetsTalk.Interfaces;
using LetsTalk.Models;
using LetsTalk.Models.Roles;
using MediatR;

namespace LetsTalk.Commands;

public sealed class RoleRequestHandler : IRequestHandler<RoleGetRequest, RoleGetResponse>,
                                         IRequestHandler<RolePostRequest>,
                                         IRequestHandler<RolePutRequest>,
                                         IRequestHandler<RoleDeleteRequest>,
                                         IRequestHandler<RoleUserGetRequest, RoleUserGetResponse>,
                                         IRequestHandler<RoleUserPutRequest>
{
    private readonly IRepository<Role> _roleRepository;
    private readonly IRepository<User> _userRepository;

    public RoleRequestHandler(IRepository<Role> roleRepository, IRepository<User> userRepository)
    {
        _roleRepository = roleRepository;
        _userRepository = userRepository;
    }

    public async Task<RoleGetResponse> Handle(RoleGetRequest request, CancellationToken cancellationToken)
    {
        if (request.RoleId is null)
            return new RoleGetResponse { Roles = new List<Role>(await _roleRepository.GetAllAsync()) };

        var role = await _roleRepository.GetAsync(request.RoleId);
        if (role is null)
            throw new NotFoundException($"Role {request.RoleId} does not exist");
        return new RoleGetResponse { Roles = { role } };
    }

    public async Task<Unit> Handle(RolePostRequest request, CancellationToken cancellationToken)
    {
        var role = await _roleRepository.GetAsync(request.Role.Id);
        if (role is not null)
            throw new ConflictException($"Role {request.Role.Id} already exists");
        await _roleRepository.InsertAsync(request.Role);
        return Unit.Value;
    }

    public async Task<Unit> Handle(RolePutRequest request, CancellationToken cancellationToken)
    {
        var role = await _roleRepository.GetAsync(request.Role.Id);
        if (role is null)
            throw new NotFoundException($"Role {request.Role.Id} does not exist");
        await _roleRepository.UpdateAsync(role);
        return Unit.Value;
    }

    public async Task<Unit> Handle(RoleDeleteRequest request, CancellationToken cancellationToken)
    {
        var role = await _roleRepository.GetAsync(request.RoleId);
        if (role is null)
            throw new NotFoundException($"Role {request.RoleId} does not exist");
        await _roleRepository.DeleteAsync(request.RoleId);
        return Unit.Value;
    }

    public async Task<RoleUserGetResponse> Handle(RoleUserGetRequest request, CancellationToken cancellationToken)
    {
        var role = await _roleRepository.GetAsync(request.RoleId);
        if (role is null)
            throw new NotFoundException($"Role {request.RoleId} does not exist");
        var users = (await _userRepository.GetAllAsync()).Where(user => user.Roles.Contains(role.Id));
        return new RoleUserGetResponse { Users = new(users) };
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
