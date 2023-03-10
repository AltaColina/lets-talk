﻿using Ardalis.Specification;
using AutoMapper;
using LetsTalk.Exceptions;
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
        var identity = new ClaimsIdentity();
        identity.AddClaim(new Claim(CustomClaims.Id, user.Id));
        identity.AddClaim(new Claim(CustomClaims.Username, user.Name));
        if (user.ImageUrl is not null)
            identity.AddClaim(new Claim(CustomClaims.ImageUrl, user.ImageUrl));
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

    private async Task<Authentication> LoginKnownUserAsync(User user)
    {
        var identity = GetIdentity(user);
        var accessToken = _tokenProvider.GenerateAccessToken(identity, out var accessTokenExpiresIn);
        var refreshToken = _tokenProvider.GenerateRefreshToken(identity, out var refreshTokenExpiresIn);
        var permissions = await GetPermissions(user);

        user.LastLoginTime = DateTime.UtcNow;
        user.RefreshTokens.RemoveAll(token => token.ExpiresIn < user.LastLoginTime);
        user.RefreshTokens.Add(new RefreshToken { Value = refreshToken, ExpiresIn = refreshTokenExpiresIn });
        await _userRepository.UpdateAsync(user);

        return new Authentication
        {
            User = _mapper.Map<UserDto>(user),
            AccessToken = accessToken,
            AccessTokenExpiresIn = accessTokenExpiresIn,
            RefreshToken = refreshToken,
            RefreshTokenExpiresIn = refreshTokenExpiresIn,
            Permissions = permissions
        };
    }

    public async Task<Authentication> AuthenticateAsync(RegisterCommand request)
    {
        if ((await _userRepository.GetByNameAsync(request.Username)) is not null)
            throw ExceptionFor<User>.AlreadyExists(u => u.Name, request.Username);

        var user = await _userRepository.AddAsync(new User
        {
            Name = request.Username,
            Secret = _passwordHandler.Encrypt(request.Password, request.Username),
            Roles = { "user" }
        });

        return await LoginKnownUserAsync(user);
    }

    public async Task<Authentication> AuthenticateAsync(LoginCommand request)
    {
        var user = await _userRepository.GetByNameAsync(request.Username);
        if (user is null || !_passwordHandler.IsValid(user.Secret, request.Username, request.Password))
            throw ExceptionFor<User>.Forbidden();

        return await LoginKnownUserAsync(user);
    }

    public async Task<Authentication> AuthenticateAsync(RefreshCommand request)
    {
        var user = await _userRepository.GetByNameAsync(request.Username);
        if (user is null || user.RefreshTokens.SingleOrDefault(token => token.Value == request.RefreshToken) is not RefreshToken token || token.ExpiresIn < DateTimeOffset.UtcNow)
            throw ExceptionFor<User>.Forbidden();

        return await LoginKnownUserAsync(user);
    }
}