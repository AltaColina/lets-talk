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

    public string GenerateAccessToken(ClaimsIdentity identity, out DateTimeOffset expiresIn)
    {
        var issuedDateTime = DateTime.UtcNow;
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = identity,
            Expires = issuedDateTime.Add(_accessTokenExpireTime),
            SigningCredentials = _signingCredentials,
        };
        var securityToken = _tokenHandler.CreateToken(tokenDescriptor);

        expiresIn = (DateTimeOffset)tokenDescriptor.Expires;

        return _tokenHandler.WriteToken(securityToken);
    }

    public string GenerateRefreshToken(ClaimsIdentity identity, out DateTimeOffset expiresIn)
    {
        Span<byte> buffer = stackalloc byte[_refreshTokenLength];
        RandomNumberGenerator.Fill(buffer);

        expiresIn = DateTimeOffset.UtcNow.Add(_refreshTokenExpireTime);

        return Convert.ToBase64String(buffer);
    }
}
