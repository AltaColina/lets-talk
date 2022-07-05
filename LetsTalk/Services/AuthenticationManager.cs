using LetsTalk.Exceptions;
using LetsTalk.Interfaces;
using LetsTalk.Models;
using System.Security.Claims;
using System.Security.Cryptography;

namespace LetsTalk.Services;

public sealed class AuthenticationManager : IAuthenticationManager
{
    private readonly ITokenProvider _tokenProvider;
    private readonly IPasswordHandler _passwordHandler;
    private readonly IUserRepository _userRepository;

    public AuthenticationManager(ITokenProvider tokenProvider, IPasswordHandler passwordHandler, IUserRepository userRepository)
    {
        _tokenProvider = tokenProvider;
        _passwordHandler = passwordHandler;
        _userRepository = userRepository;
    }

    private static ClaimsIdentity GetIdentity(User user)
    {
        ClaimsIdentity identity = new ClaimsIdentity();
        identity.AddClaim(new Claim(ClaimTypes.Name, user.Id));
        foreach (var role in user.Roles)
            identity.AddClaim(new Claim(ClaimTypes.Role, role.ToString()));
        return identity;
    }

    private static string GenerateRefreshToken()
    {
        Span<byte> buffer = stackalloc byte[32];
        RandomNumberGenerator.Fill(buffer);
        return Convert.ToBase64String(buffer);
    }

    public async Task<AuthenticationResponse> AuthenticateAsync(RegisterRequest request)
    {
        if ((await _userRepository.GetAsync(request.Username)) is not null)
            throw new ConflictException($"Username '{request.Username}' already in use");

        var creationDateTime = DateTime.UtcNow;
        var user = new User
        {
            Id = request.Username,
            Secret = _passwordHandler.Encrypt(request.Password, request.Username),
            CreationTime = creationDateTime,
            LastLoginTime = creationDateTime,
            Roles = { "User" }
        };

        var identity = GetIdentity(user);
        var accessToken = _tokenProvider.GenerateAccessToken(identity);
        var refreshToken = _tokenProvider.GenerateRefreshToken(identity);
        
        user.RefreshTokens.Add(refreshToken);
        await _userRepository.InsertAsync(user);

        return new AuthenticationResponse
        {
            Person = new Person { Username = request.Username },
            AccessToken = accessToken,
            RefreshToken = refreshToken,
        };
    }

    public async Task<AuthenticationResponse> AuthenticateAsync(LoginRequest request)
    {
        var user = await _userRepository.GetAsync(request.Username);
        if (user is null || !_passwordHandler.IsValid(user.Secret, request.Username, request.Password))
            throw new ForbiddenException("Incorrect username or password");

        var identity = GetIdentity(user);
        var accessToken = _tokenProvider.GenerateAccessToken(identity);
        var refreshToken = _tokenProvider.GenerateRefreshToken(identity);

        user.LastLoginTime = DateTime.UtcNow;
        user.RefreshTokens.RemoveAll(token => token.ExpiresIn < user.LastLoginTime);
        user.RefreshTokens.Add(refreshToken);
        await _userRepository.UpdateAsync(user);

        return new AuthenticationResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            Person = new Person { Username = identity.Name }
        };
    }

    public async Task<AuthenticationResponse> AuthenticateAsync(RefreshRequest request)
    {
        var user = await _userRepository.GetAsync(request.Username);
        if (user is null || user.RefreshTokens.SingleOrDefault(token => token.Id == request.RefreshToken) is not Token token || token.ExpiresIn < DateTimeOffset.UtcNow)
            throw new ForbiddenException("Incorrect username or refresh token");

        var identity = GetIdentity(user);
        var accessToken = _tokenProvider.GenerateAccessToken(identity);
        var refreshToken = _tokenProvider.GenerateRefreshToken(identity);

        user.LastLoginTime = DateTime.UtcNow;
        user.RefreshTokens.RemoveAll(token => token.ExpiresIn < user.LastLoginTime);
        user.RefreshTokens.Add(refreshToken);
        await _userRepository.UpdateAsync(user);

        return new AuthenticationResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            Person = new Person { Username = identity.Name }
        };
    }
}
