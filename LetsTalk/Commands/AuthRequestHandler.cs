using LetsTalk.Dtos.Auths;
using LetsTalk.Interfaces;
using LetsTalk.Models;
using MediatR;

namespace LetsTalk.Commands;

public sealed class AuthRequestHandler : IRequestHandler<RegisterRequest, Authentication>,
                                         IRequestHandler<LoginRequest, Authentication>,
                                         IRequestHandler<RefreshRequest, Authentication>
{
    private readonly IAuthenticationManager _authenticationManager;

    public AuthRequestHandler(IAuthenticationManager authenticationManager)
    {
        _authenticationManager = authenticationManager;
    }

    public async Task<Authentication> Handle(RegisterRequest request, CancellationToken cancellationToken)
    {
        return await _authenticationManager.AuthenticateAsync(request);
    }

    public async Task<Authentication> Handle(LoginRequest request, CancellationToken cancellationToken)
    {
        return await _authenticationManager.AuthenticateAsync(request);
    }

    public async Task<Authentication> Handle(RefreshRequest request, CancellationToken cancellationToken)
    {
        return await _authenticationManager.AuthenticateAsync(request);
    }
}
