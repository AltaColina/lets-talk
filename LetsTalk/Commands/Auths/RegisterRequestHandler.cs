using LetsTalk.Interfaces;
using LetsTalk.Models.Auths;
using MediatR;

namespace LetsTalk.Commands.Auths;

public sealed class RegisterRequestHandler : IRequestHandler<RegisterRequest, AuthenticationResponse>
{
    private readonly IAuthenticationManager _authenticationManager;

    public RegisterRequestHandler(IAuthenticationManager authenticationManager)
    {
        _authenticationManager = authenticationManager;
    }

    public async Task<AuthenticationResponse> Handle(RegisterRequest request, CancellationToken cancellationToken)
    {
        return await _authenticationManager.AuthenticateAsync(request);
    }
}
