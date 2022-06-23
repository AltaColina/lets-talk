using LetsTalk.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace LetsTalk.Services;

public sealed class JwtAuthenticationManager : IAuthenticationManager
{
    private readonly SigningCredentials _signingCredentials;
    private readonly JwtSecurityTokenHandler _tokenHandler;
    private readonly IPasswordHandler _passwordHandler;
    private readonly IUserRepository _userRepository;
    private readonly TimeSpan _accessTokenExpireTime;
    private readonly TimeSpan _refreshTokenExpireTime;

    public JwtAuthenticationManager(SecurityKey securityKey, IPasswordHandler passwordHandler, IUserRepository userRepository)
    {
        _signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);
        _tokenHandler = new JwtSecurityTokenHandler();
        _passwordHandler = passwordHandler;
        _userRepository = userRepository;
        _accessTokenExpireTime = TimeSpan.FromHours(1);
        _refreshTokenExpireTime = TimeSpan.FromDays(1);
    }

    private static ClaimsIdentity GetIdentity(User user)
    {
        return new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.Name, user.Id)
        });
    }

    private static string GenerateRefreshToken()
    {
        Span<byte> buffer = stackalloc byte[32];
        RandomNumberGenerator.Fill(buffer);
        return Convert.ToBase64String(buffer);
    }

    public Token GenerateToken(User user)
    {
        var issuedDateTime = DateTime.UtcNow;
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = GetIdentity(user),
            Expires = issuedDateTime.Add(_accessTokenExpireTime),
            SigningCredentials = _signingCredentials,
        };
        var securityToken = _tokenHandler.CreateToken(tokenDescriptor);
        var issuedDateTimeUnix = issuedDateTime - DateTime.UnixEpoch;
        return new Token
        {
            AccessToken = _tokenHandler.WriteToken(securityToken),
            ExpiresIn = (int)issuedDateTimeUnix.Add(_accessTokenExpireTime).TotalSeconds,
            RefreshToken = GenerateRefreshToken(),
            RefreshTokenExpiresIn = (int)issuedDateTimeUnix.Add(_refreshTokenExpireTime).TotalSeconds,
        };
    }

    public Token? Authenticate(string username, string password)
    {
        if (String.IsNullOrWhiteSpace(username) || String.IsNullOrWhiteSpace(password))
            return null;

        var user = _userRepository.Get(username);
        if (user is null || !_passwordHandler.IsValid(user.Secret, password, username))
            return null;

        return GenerateToken(user);
    }

    public Token? Refresh(string username, string refreshToken)
    {
        if (String.IsNullOrWhiteSpace(username) || String.IsNullOrWhiteSpace(refreshToken))
            return null;

        var user = _userRepository.Get(username);
        if (user is null || user.RefreshTokens.SingleOrDefault(token => token.Id == refreshToken) is not RefreshToken token)
            return null;

        if (token.ExpiresIn < DateTime.UtcNow)
            return null;

        return GenerateToken(user);
    }
}
