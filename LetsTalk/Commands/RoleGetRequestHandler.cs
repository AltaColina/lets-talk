using LetsTalk.Exceptions;
using LetsTalk.Interfaces;
using LetsTalk.Models;
using MediatR;

namespace LetsTalk.Commands;

public sealed class RoleGetRequestHandler : IRequestHandler<RoleGetRequest, RoleGetResponse>
{
    private readonly IUserRepository _userRepository;

    public RoleGetRequestHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<RoleGetResponse> Handle(RoleGetRequest request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetAsync(request.UserId);
        if (user is null)
            throw new NotFoundException($"User {request.UserId} does not exist");
        return new RoleGetResponse
        {
            Roles = new List<Role>(user.Roles)
        };
    }
}
