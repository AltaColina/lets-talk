using LetsTalk.Exceptions;
using LetsTalk.Interfaces;
using LetsTalk.Models;
using MediatR;

namespace LetsTalk.Commands;

public sealed class RolePutRequestHandler : IRequestHandler<RolePutRequest>
{
    private readonly IUserRepository _userRepository;

    public RolePutRequestHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Unit> Handle(RolePutRequest request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetAsync(request.UserId);
        if(user is null)
            throw new NotFoundException($"User {request.UserId} does not exist");
        user.Roles.UnionWith(request.Roles);
        await _userRepository.UpdateAsync(user);
        return Unit.Value;
    }
}
