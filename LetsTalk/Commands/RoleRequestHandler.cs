using Ardalis.Specification;
using AutoMapper;
using LetsTalk.Dtos.Chats;
using LetsTalk.Dtos.Roles;
using LetsTalk.Dtos.Users;
using LetsTalk.Exceptions;
using LetsTalk.Interfaces;
using LetsTalk.Models;
using MediatR;

namespace LetsTalk.Commands;

public sealed class RoleRequestHandler : IRequestHandler<GetRolesRequest, GetRolesResponse>,
                                         IRequestHandler<GetRoleByIdRequest, RoleDto>,
                                         IRequestHandler<CreateRoleRequest, RoleDto>,
                                         IRequestHandler<UpdateRoleRequest>,
                                         IRequestHandler<DeleteRoleRequest>,
                                         IRequestHandler<GetRoleUsersRequest, GetRoleUsersResponse>
{
    private readonly IMapper _mapper;
    private readonly IRepository<Role> _roleRepository;
    private readonly IRepository<User> _userRepository;

    public RoleRequestHandler(IMapper mapper, IRepository<Role> roleRepository, IRepository<User> userRepository)
    {
        _mapper = mapper;
        _roleRepository = roleRepository;
        _userRepository = userRepository;
    }

    public async Task<GetRolesResponse> Handle(GetRolesRequest request, CancellationToken cancellationToken)
    {
        return new GetRolesResponse { Roles = _mapper.Map<List<RoleDto>>(await _roleRepository.ListAsync(cancellationToken)) };
    }

    public async Task<RoleDto> Handle(GetRoleByIdRequest request, CancellationToken cancellationToken)
    {
        var role = await _roleRepository.GetByIdAsync(request.Id, cancellationToken);
        if (role is null)
            throw new NotFoundException($"Role {request.Id} does not exist");
        return _mapper.Map<RoleDto>(role);
    }

    public async Task<RoleDto> Handle(CreateRoleRequest request, CancellationToken cancellationToken)
    {
        if ((await _roleRepository.GetByIdAsync(request.Id, cancellationToken)) is not null)
            throw new ConflictException($"Role {request.Id} already exists");
        var role = _mapper.Map<Role>(request);
        role.Name ??= role.Id;
        role = await _roleRepository.AddAsync(role, cancellationToken);
        return _mapper.Map<RoleDto>(role);
    }

    public async Task<Unit> Handle(UpdateRoleRequest request, CancellationToken cancellationToken)
    {
        var role = await _roleRepository.GetByIdAsync(request.Id, cancellationToken);
        if (role is null)
            throw new NotFoundException($"Role {request.Id} does not exist");
        role = _mapper.Map(request, role);
        await _roleRepository.UpdateAsync(role, cancellationToken);
        return Unit.Value;
    }

    public async Task<Unit> Handle(DeleteRoleRequest request, CancellationToken cancellationToken)
    {
        var role = await _roleRepository.GetByIdAsync(request.Id, cancellationToken);
        if (role is null)
            throw new NotFoundException($"Role {request.Id} does not exist");
        await _roleRepository.DeleteAsync(role);
        return Unit.Value;
    }

    private sealed class GetRoleUsersRequestSpecification : Specification<User>
    {
        public GetRoleUsersRequestSpecification(string roleId)
        {
            Query.Where(user => user.Roles.Contains(roleId));
        }
    }

    public async Task<GetRoleUsersResponse> Handle(GetRoleUsersRequest request, CancellationToken cancellationToken)
    {
        var role = await _roleRepository.GetByIdAsync(request.Id, cancellationToken);
        if (role is null)
            throw new NotFoundException($"Role {request.Id} does not exist");

        var users = await _userRepository.ListAsync(new GetRoleUsersRequestSpecification(request.Id), cancellationToken);
        return new GetRoleUsersResponse { Users = _mapper.Map<List<UserDto>>(users) };
    }
}
