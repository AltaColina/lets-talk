using LetsTalk.Exceptions;
using LetsTalk.Interfaces;
using LetsTalk.Models;
using MediatR;
using System.Text.RegularExpressions;

namespace LetsTalk.Commands;

public sealed class RegisterRequestHandler : IRequestHandler<RegisterRequest, AuthenticationResponse>
{
    private static readonly Regex UsernameRegex = new("^[a-zA-Z][a-z0-9_-]{3,15}$");
    private readonly IUserRepository _userRepository;
    private readonly IAuthenticationManager _authenticationManager;
    private readonly IPasswordHandler _passwordHandler;

    public RegisterRequestHandler(IUserRepository userRepository, IAuthenticationManager authenticationManager, IPasswordHandler passwordHandler)
    {
        _userRepository = userRepository;
        _authenticationManager = authenticationManager;
        _passwordHandler = passwordHandler;
    }

    public async Task<AuthenticationResponse> Handle(RegisterRequest request, CancellationToken cancellationToken)
    {
        return await _authenticationManager.AuthenticateAsync(request);
    }
}
