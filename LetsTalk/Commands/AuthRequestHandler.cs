using LetsTalk.Interfaces;
using LetsTalk.Models.Auths;
using MediatR;

namespace LetsTalk.Commands;

public sealed class AuthRequestHandler : IRequestHandler<RegisterRequest, AuthenticationResponse>,
                                         IRequestHandler<LoginRequest, AuthenticationResponse>,
                                         IRequestHandler<RefreshRequest, AuthenticationResponse>
{
    private readonly IAuthenticationManager _authenticationManager;

    public AuthRequestHandler(IAuthenticationManager authenticationManager)
    {
        _authenticationManager = authenticationManager;
    }

    public async Task<AuthenticationResponse> Handle(RegisterRequest request, CancellationToken cancellationToken)
    {
        return await _authenticationManager.AuthenticateAsync(request);
    }

    public async Task<AuthenticationResponse> Handle(LoginRequest request, CancellationToken cancellationToken)
    {
        return await _authenticationManager.AuthenticateAsync(request);
    }

    public async Task<AuthenticationResponse> Handle(RefreshRequest request, CancellationToken cancellationToken)
    {
        return await _authenticationManager.AuthenticateAsync(request);
    }
}
