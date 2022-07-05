using LetsTalk.Exceptions;
using LetsTalk.Interfaces;
using LetsTalk.Models;
using MediatR;

namespace LetsTalk.Commands;

public sealed class UserPutRequestHandler : IRequestHandler<UserPutRequest>
{
    private readonly IUserRepository _userRepository;

    public UserPutRequestHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Unit> Handle(UserPutRequest request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetAsync(request.User.Id);
        if (user is null)
            throw new NotFoundException($"User {request.User.Id} does not exist");
        await _userRepository.UpdateAsync(request.User);
        return Unit.Value;
    }
}