using LetsTalk.Exceptions;
using LetsTalk.Interfaces;
using LetsTalk.Models;
using MediatR;

namespace LetsTalk.Commands;

public sealed class RolePostRequestHandler : IRequestHandler<RolePostRequest>
{
    private readonly IRoleRepository _roleRepository;

    public RolePostRequestHandler(IRoleRepository roleRepository)
    {
        _roleRepository = roleRepository;
    }

    public async Task<Unit> Handle(RolePostRequest request, CancellationToken cancellationToken)
    {
        var role = await _roleRepository.GetAsync(request.Role.Id);
        if (role is not null)
            throw new ConflictException($"Role {request.Role.Id} already exists");
        await _roleRepository.InsertAsync(request.Role);
        return Unit.Value;
    }
}
