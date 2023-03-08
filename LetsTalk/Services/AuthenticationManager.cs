using Ardalis.Specification;
using AutoMapper;
using LetsTalk.Exceptions;
using LetsTalk.Interfaces;
using LetsTalk.Repositories;
using LetsTalk.Roles;
using LetsTalk.Security;
using LetsTalk.Security.Commands;
using LetsTalk.Users;
using System.Security.Claims;
using System.Security.Cryptography;

namespace LetsTalk.Services;

internal sealed class AuthenticationManager : IAuthenticationManager
{
    private readonly IMapper _mapper;
    private readonly ITokenProvider _tokenProvider;
    private readonly IPasswordHandler _passwordHandler;
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;

    public AuthenticationManager(IMapper mapper, ITokenProvider tokenProvider, IPasswordHandler passwordHandler, IUserRepository userRepository, IRoleRepository roleRepository)
    {
        _mapper = mapper;
        _tokenProvider = tokenProvider;
        _passwordHandler = passwordHandler;
        _userRepository = userRepository;
        _roleRepository = roleRepository;
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

    private sealed class GetRolesInCollectionSpecification : Specification<Role>
    {
        public GetRolesInCollectionSpecification(ICollection<string> roleIds)
        {
            Query.Where(role => roleIds.Contains(role.Id));
        }
    }

    private async Task<HashSet<string>> GetPermissions(User user)
    {
        var roles = await _roleRepository.ListAsync(new GetRolesInCollectionSpecification(user.Roles));
        var permissions = roles.Select(r => r.Permissions).Aggregate((p, c) => { p.UnionWith(c); return p; });
        return permissions;
    }

    public async Task<Authentication> AuthenticateAsync(RegisterCommand request)
    {
        if ((await _userRepository.GetByIdAsync(request.Username)) is not null)
            throw new ConflictException($"Username '{request.Username}' already in use");

        var creationDateTime = DateTime.UtcNow;
        var user = new User
        {
            Id = request.Username,
            Name = request.Name ?? request.Username,
            Secret = _passwordHandler.Encrypt(request.Password, request.Username),
            CreationTime = creationDateTime,
            LastLoginTime = creationDateTime,
            Roles = { "user" }
        };

        var identity = GetIdentity(user);
        var accessToken = _tokenProvider.GenerateAccessToken(identity);
        var refreshToken = _tokenProvider.GenerateRefreshToken(identity);
        var permissions = await GetPermissions(user);

        user.RefreshTokens.Add(refreshToken);
        await _userRepository.AddAsync(user);

        return new Authentication
        {
            User = _mapper.Map<UserDto>(user),
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            Permissions = permissions
        };
    }

    public async Task<Authentication> AuthenticateAsync(LoginCommand request)
    {
        var user = await _userRepository.GetByIdAsync(request.Username);
        if (user is null || !_passwordHandler.IsValid(user.Secret, request.Username, request.Password))
            throw new ForbiddenException("Incorrect username or password");

        var identity = GetIdentity(user);
        var accessToken = _tokenProvider.GenerateAccessToken(identity);
        var refreshToken = _tokenProvider.GenerateRefreshToken(identity);
        var permissions = await GetPermissions(user);

        user.LastLoginTime = DateTime.UtcNow;
        user.RefreshTokens.RemoveAll(token => token.ExpiresIn < user.LastLoginTime);
        user.RefreshTokens.Add(refreshToken);
        await _userRepository.UpdateAsync(user);

        return new Authentication
        {
            User = _mapper.Map<UserDto>(user),
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            Permissions = permissions
        };
    }

    public async Task<Authentication> AuthenticateAsync(RefreshCommand request)
    {
        var user = await _userRepository.GetByIdAsync(request.Username);
        if (user is null || user.RefreshTokens.SingleOrDefault(token => token.Id == request.RefreshToken) is not Token token || token.ExpiresIn < DateTimeOffset.UtcNow)
            throw new ForbiddenException("Incorrect username or refresh token");

        var identity = GetIdentity(user);
        var accessToken = _tokenProvider.GenerateAccessToken(identity);
        var refreshToken = _tokenProvider.GenerateRefreshToken(identity);
        var permissions = await GetPermissions(user);

        user.LastLoginTime = DateTime.UtcNow;
        user.RefreshTokens.RemoveAll(token => token.ExpiresIn < user.LastLoginTime);
        user.RefreshTokens.Add(refreshToken);
        await _userRepository.UpdateAsync(user);

        return new Authentication
        {
            User = _mapper.Map<UserDto>(user),
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            Permissions = permissions
        };
    }
}
