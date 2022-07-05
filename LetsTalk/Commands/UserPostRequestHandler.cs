using LetsTalk.Exceptions;
using LetsTalk.Interfaces;
using LetsTalk.Models;
using MediatR;

namespace LetsTalk.Commands;

public sealed class UserPostRequestHandler : IRequestHandler<UserPostRequest>
{
    private readonly IUserRepository _userRepository;

    public UserPostRequestHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Unit> Handle(UserPostRequest request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetAsync(request.User.Id);
        if (user is not null)
            throw new ConflictException($"User {request.User.Id} already exists");
        await _userRepository.InsertAsync(request.User);
        return Unit.Value;
    }
}
