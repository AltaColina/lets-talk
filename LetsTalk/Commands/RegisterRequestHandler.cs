using LetsTalk.Exceptions;
using LetsTalk.Interfaces;
using LetsTalk.Models;
using MediatR;
using System.Text.RegularExpressions;

namespace LetsTalk.Commands;

public sealed class RegisterRequestHandler : IRequestHandler<RegisterRequest, RegisterResponse>
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

    public async Task<RegisterResponse> Handle(RegisterRequest request, CancellationToken cancellationToken)
    {
        if (!UsernameRegex.IsMatch(request.Username))
            throw new ArgumentException("Invalid username");

        // TODO: Validate password?

        if ((await _userRepository.GetAsync(request.Username)) is not null)
            throw new ConflictException("Username already in use");

        var creationDateTime = DateTime.UtcNow;
        var user = new User
        {
            Id = request.Username,
            Secret = _passwordHandler.Encrypt(request.Password, request.Username),
            CreationTime = creationDateTime,
            LastLoginTime = creationDateTime,
        };

        var token = _authenticationManager.GenerateToken(user);

        user.RefreshTokens.Add(new RefreshToken
        {
            Id = token.RefreshToken,
            ExpiresIn = DateTime.UnixEpoch.AddSeconds(token.RefreshTokenExpiresIn)
        });
        await _userRepository.InsertAsync(user);

        return new RegisterResponse
        {
            Person = new Person { Username = request.Username },
            Token = token
        };
    }
}
