using AutoMapper;
using LetsTalk.Dtos.Auths;
using LetsTalk.Dtos.Users;
using LetsTalk.Exceptions;
using LetsTalk.Interfaces;
using LetsTalk.Models;
using System.Security.Claims;
using System.Security.Cryptography;

namespace LetsTalk.Services;

internal sealed class AuthenticationManager : IAuthenticationManager
{
    private readonly IMapper _mapper;
    private readonly ITokenProvider _tokenProvider;
    private readonly IPasswordHandler _passwordHandler;
    private readonly IRepository<User> _userRepository;

    public AuthenticationManager(IMapper mapper, ITokenProvider tokenProvider, IPasswordHandler passwordHandler, IRepository<User> userRepository)
    {
        _mapper = mapper;
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

    public async Task<Authentication> AuthenticateAsync(RegisterRequest request)
    {
        if ((await _userRepository.GetByIdAsync(request.Username)) is not null)
            throw new ConflictException($"Username '{request.Username}' already in use");

        var creationDateTime = DateTime.UtcNow;
        var user = new User
        {
            Id = request.Username,
            Secret = _passwordHandler.Encrypt(request.Password, request.Username),
            CreationTime = creationDateTime,
            LastLoginTime = creationDateTime,
            Roles = { "user" }
        };

        var identity = GetIdentity(user);
        var accessToken = _tokenProvider.GenerateAccessToken(identity);
        var refreshToken = _tokenProvider.GenerateRefreshToken(identity);
        
        user.RefreshTokens.Add(refreshToken);
        await _userRepository.AddAsync(user);

        return new Authentication
        {
            User = _mapper.Map<UserDto>(user),
            AccessToken = accessToken,
            RefreshToken = refreshToken,
        };
    }

    public async Task<Authentication> AuthenticateAsync(LoginRequest request)
    {
        var user = await _userRepository.GetByIdAsync(request.Username);
        if (user is null || !_passwordHandler.IsValid(user.Secret, request.Username, request.Password))
            throw new ForbiddenException("Incorrect username or password");

        var identity = GetIdentity(user);
        var accessToken = _tokenProvider.GenerateAccessToken(identity);
        var refreshToken = _tokenProvider.GenerateRefreshToken(identity);

        user.LastLoginTime = DateTime.UtcNow;
        user.RefreshTokens.RemoveAll(token => token.ExpiresIn < user.LastLoginTime);
        user.RefreshTokens.Add(refreshToken);
        await _userRepository.UpdateAsync(user);

        return new Authentication
        {
            User = _mapper.Map<UserDto>(user),
            AccessToken = accessToken,
            RefreshToken = refreshToken,
        };
    }

    public async Task<Authentication> AuthenticateAsync(RefreshRequest request)
    {
        var user = await _userRepository.GetByIdAsync(request.Username);
        if (user is null || user.RefreshTokens.SingleOrDefault(token => token.Id == request.RefreshToken) is not Token token || token.ExpiresIn < DateTimeOffset.UtcNow)
            throw new ForbiddenException("Incorrect username or refresh token");

        var identity = GetIdentity(user);
        var accessToken = _tokenProvider.GenerateAccessToken(identity);
        var refreshToken = _tokenProvider.GenerateRefreshToken(identity);

        user.LastLoginTime = DateTime.UtcNow;
        user.RefreshTokens.RemoveAll(token => token.ExpiresIn < user.LastLoginTime);
        user.RefreshTokens.Add(refreshToken);
        await _userRepository.UpdateAsync(user);

        return new Authentication
        {
            User = _mapper.Map<UserDto>(user),
            AccessToken = accessToken,
            RefreshToken = refreshToken,
        };
    }
}
