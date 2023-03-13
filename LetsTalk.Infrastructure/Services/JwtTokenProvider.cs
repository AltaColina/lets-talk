using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace LetsTalk.Services;

internal sealed class JwtTokenProvider : IJwtTokenProvider
{
    private readonly SigningCredentials _signingCredentials;
    private readonly JsonWebTokenHandler _tokenHandler;
    private readonly TimeSpan _accessTokenExpireTime;
    private readonly TimeSpan _refreshTokenExpireTime;

    public JwtTokenProvider(IConfiguration configuration)
    {
        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetRequiredSection("SigningKey").Value!));
        _signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256Signature);
        _tokenHandler = new JsonWebTokenHandler();
        _accessTokenExpireTime = TimeSpan.FromHours(1);
        _refreshTokenExpireTime = TimeSpan.FromDays(1);
    }

    public string CreateAccessToken(ClaimsIdentity identity, out DateTime expires)
    {
        var issuedAt = DateTime.UtcNow;
        expires = issuedAt.Add(_accessTokenExpireTime);
        return _tokenHandler.CreateToken(new SecurityTokenDescriptor
        {
            Subject = identity,
            IssuedAt = issuedAt,
            NotBefore = issuedAt,
            Expires = expires,
            SigningCredentials = _signingCredentials,
        });
    }

    public JsonWebToken ReadAccessToken(string accessToken)
    {
        return _tokenHandler.ReadJsonWebToken(accessToken);
    }

    public string CreateRefreshToken(ClaimsIdentity identity, out DateTime expires)
    {
        var issuedAt = DateTime.UtcNow;
        expires = issuedAt.Add(_refreshTokenExpireTime);
        var name = identity.Claims.Single(c => c.Type == ClaimTypes.Name).Value;
        var payload = $$"""{"{{JwtRegisteredClaimNames.Name}}":{{name}},"{{JwtRegisteredClaimNames.Nbf}}:{{issuedAt}},"{{JwtRegisteredClaimNames.Exp}}: {{expires}},"{{JwtRegisteredClaimNames.Iss}}: {{issuedAt}}}""";
        return Convert.ToBase64String(Encoding.UTF8.GetBytes(payload));
    }

    public JsonWebToken ReadRefreshToken(string refreshToken)
    {
        var payload = Encoding.UTF8.GetString(Convert.FromBase64String(refreshToken));
        return new JsonWebToken("{}", payload);
    }
}