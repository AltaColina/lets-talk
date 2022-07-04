using LetsTalk.Exceptions;
using LetsTalk.Interfaces;
using LetsTalk.Models;
using MediatR;

namespace LetsTalk.Commands;

public sealed class RefreshRequestHandler : IRequestHandler<RefreshRequest, AuthenticationResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IAuthenticationManager _authenticationManager;

    public RefreshRequestHandler(IUserRepository userRepository, IAuthenticationManager authenticationManager)
    {
        _userRepository = userRepository;
        _authenticationManager = authenticationManager;
    }

    public async Task<AuthenticationResponse> Handle(RefreshRequest request, CancellationToken cancellationToken)
    {
        return await _authenticationManager.AuthenticateAsync(request);
    }
}