using LetsTalk.Exceptions;
using LetsTalk.Interfaces;
using LetsTalk.Models;
using LetsTalk.Models.Users;
using MediatR;

namespace LetsTalk.Commands.Users;

public sealed class UserGetRequestHandler : IRequestHandler<UserGetRequest, UserGetResponse>
{
    private readonly IUserRepository _userRepository;

    public UserGetRequestHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserGetResponse> Handle(UserGetRequest request, CancellationToken cancellationToken)
    {
        if (String.IsNullOrWhiteSpace(request.UserId))
            return new UserGetResponse { Users = new List<User>(await _userRepository.GetAllAsync()) };

        var user = await _userRepository.GetAsync(request.UserId);
        if (user is null)
            throw new NotFoundException($"User {request.UserId} does not exist");
        return new UserGetResponse { Users = { user } };
    }
}
