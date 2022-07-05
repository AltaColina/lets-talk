using LetsTalk.Exceptions;
using LetsTalk.Interfaces;
using LetsTalk.Models;
using MediatR;

namespace LetsTalk.Commands;

public sealed class RoleDeleteRequestHandler : IRequestHandler<RoleDeleteRequest>
{
    private readonly IRoleRepository _roleRepository;

    public RoleDeleteRequestHandler(IRoleRepository roleRepository)
    {
        _roleRepository = roleRepository;
    }

    public async Task<Unit> Handle(RoleDeleteRequest request, CancellationToken cancellationToken)
    {
        var role = await _roleRepository.GetAsync(request.RoleId);
        if (role is null)
            throw new NotFoundException($"Role {request.RoleId} does not exist");
        await _roleRepository.DeleteAsync(request.RoleId);
        return Unit.Value;
    }
}
