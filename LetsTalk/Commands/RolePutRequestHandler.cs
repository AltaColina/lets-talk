using LetsTalk.Exceptions;
using LetsTalk.Interfaces;
using LetsTalk.Models;
using MediatR;

namespace LetsTalk.Commands;

public sealed class RolePutRequestHandler : IRequestHandler<RolePutRequest>
{
    private readonly IRoleRepository _roleRepository;

    public RolePutRequestHandler(IRoleRepository roleRepository)
    {
        _roleRepository = roleRepository;
    }

    public async Task<Unit> Handle(RolePutRequest request, CancellationToken cancellationToken)
    {
        var role = await _roleRepository.GetAsync(request.Role.Id);
        if (role is null)
            throw new NotFoundException($"Role {request.Role.Id} does not exist");
        await _roleRepository.UpdateAsync(role);
        return Unit.Value;
    }
}