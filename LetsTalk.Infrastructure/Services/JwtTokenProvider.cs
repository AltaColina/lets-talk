using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace LetsTalk.Services;

internal sealed class JwtTokenProvider : ITokenProvider
{
    private readonly SigningCredentials _signingCredentials;
    private readonly JwtSecurityTokenHandler _tokenHandler;
    private readonly TimeSpan _accessTokenExpireTime;
    private readonly TimeSpan _refreshTokenExpireTime;

    public JwtTokenProvider(SecurityKey securityKey)
    {
        _signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);
        _tokenHandler = new JwtSecurityTokenHandler();
        _accessTokenExpireTime = TimeSpan.FromHours(1);
        _refreshTokenExpireTime = TimeSpan.FromDays(1);
    }

    private SecurityToken CreateToken(ClaimsIdentity identity, bool isAccess, out string serializedToken)
    {
        var issuedAt = DateTime.UtcNow;
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = identity,
            IssuedAt = issuedAt,
            NotBefore = issuedAt,
            Expires = issuedAt.Add(isAccess ? _accessTokenExpireTime : _refreshTokenExpireTime)
        };
        if (isAccess)
            tokenDescriptor.SigningCredentials = _signingCredentials;

        var token = _tokenHandler.CreateToken(tokenDescriptor);

        serializedToken = _tokenHandler.WriteToken(token);

        return token;
    }

    public SecurityToken GenerateAccessToken(ClaimsIdentity identity, out string serializedToken)
    {
        return CreateToken(identity, isAccess: true, out serializedToken);
    }

    public SecurityToken GenerateRefreshToken(ClaimsIdentity identity, out string serializedToken)
    {
        return CreateToken(identity, isAccess: false, out serializedToken);
    }
}
