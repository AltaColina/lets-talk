using LetsTalk.Exceptions;
using LetsTalk.Interfaces;
using LetsTalk.Models;
using MediatR;

namespace LetsTalk.Commands;

public sealed class UserDeleteRequestHandler : IRequestHandler<UserDeleteRequest>
{
    private readonly IUserRepository _userRepository;

    public UserDeleteRequestHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Unit> Handle(UserDeleteRequest request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetAsync(request.UserId);
        if (user is null)
            throw new NotFoundException($"User {request.UserId} does not exist");
        await _userRepository.DeleteAsync(request.UserId);
        return Unit.Value;
    }
}
