using LetsTalk.Interfaces;
using LetsTalk.Models.Auths;
using MediatR;

namespace LetsTalk.Commands.Auths;

public sealed class RefreshRequestHandler : IRequestHandler<RefreshRequest, AuthenticationResponse>
{
    private readonly IAuthenticationManager _authenticationManager;

    public RefreshRequestHandler(IAuthenticationManager authenticationManager)
    {
        _authenticationManager = authenticationManager;
    }

    public async Task<AuthenticationResponse> Handle(RefreshRequest request, CancellationToken cancellationToken)
    {
        return await _authenticationManager.AuthenticateAsync(request);
    }
}