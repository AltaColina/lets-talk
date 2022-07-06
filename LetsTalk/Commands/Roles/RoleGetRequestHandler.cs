using LetsTalk.Exceptions;
using LetsTalk.Interfaces;
using LetsTalk.Models;
using LetsTalk.Models.Roles;
using MediatR;

namespace LetsTalk.Commands.Roles;

public sealed class RoleGetRequestHandler : IRequestHandler<RoleGetRequest, RoleGetResponse>
{
    private readonly IRoleRepository _roleRepository;

    public RoleGetRequestHandler(IRoleRepository roleRepository)
    {
        _roleRepository = roleRepository;
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
}
