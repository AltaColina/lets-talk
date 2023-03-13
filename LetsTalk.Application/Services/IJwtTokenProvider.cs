using Microsoft.IdentityModel.JsonWebTokens;
using System.Security.Claims;

namespace LetsTalk.Services;

public interface IJwtTokenProvider
{
    public string CreateAccessToken(ClaimsIdentity identity, out DateTime expires);
    public JsonWebToken ReadAccessToken(string accessToken);
    public string CreateRefreshToken(ClaimsIdentity identity, out DateTime expires);
    public JsonWebToken ReadRefreshToken(string refreshToken);
}
