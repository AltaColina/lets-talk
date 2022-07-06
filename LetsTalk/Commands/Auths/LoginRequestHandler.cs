using LetsTalk.Interfaces;
using LetsTalk.Models.Auths;
using MediatR;

namespace LetsTalk.Commands.Auths;

public sealed class LoginRequestHandler : IRequestHandler<LoginRequest, AuthenticationResponse>
{
    private readonly IAuthenticationManager _authenticationManager;

    public LoginRequestHandler(IAuthenticationManager authenticationManager)
    {
        _authenticationManager = authenticationManager;
    }

    public async Task<AuthenticationResponse> Handle(LoginRequest request, CancellationToken cancellationToken)
    {
        return await _authenticationManager.AuthenticateAsync(request);
    }
}
