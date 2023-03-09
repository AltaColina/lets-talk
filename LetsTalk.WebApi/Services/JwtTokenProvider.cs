using LetsTalk.Interfaces;
using LetsTalk.Security;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace LetsTalk.Services;

internal sealed class JwtTokenProvider : ITokenProvider
{
    private readonly SigningCredentials _signingCredentials;
    private readonly JwtSecurityTokenHandler _tokenHandler;
    private readonly TimeSpan _accessTokenExpireTime;
    private readonly int _refreshTokenLength;
    private readonly TimeSpan _refreshTokenExpireTime;

    public JwtTokenProvider(SecurityKey securityKey)
    {
        _signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);
        _tokenHandler = new JwtSecurityTokenHandler();
        _accessTokenExpireTime = TimeSpan.FromHours(1);
        _refreshTokenLength = 32;
        _refreshTokenExpireTime = TimeSpan.FromDays(1);
    }

    public Token GenerateAccessToken(ClaimsIdentity identity)
    {
        var issuedDateTime = DateTime.UtcNow;
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = identity,
            Expires = issuedDateTime.Add(_accessTokenExpireTime),
            SigningCredentials = _signingCredentials,
        };
        var securityToken = _tokenHandler.CreateToken(tokenDescriptor);
        return new Token
        {
            Id = _tokenHandler.WriteToken(securityToken),
            ExpiresIn = (DateTimeOffset)tokenDescriptor.Expires,
        };
    }

    public Token GenerateRefreshToken(ClaimsIdentity identity)
    {
        Span<byte> buffer = stackalloc byte[_refreshTokenLength];
        RandomNumberGenerator.Fill(buffer);
        return new Token
        {
            Id = Convert.ToBase64String(buffer),
            ExpiresIn = DateTimeOffset.UtcNow.Add(_refreshTokenExpireTime),
        };
    }
}
