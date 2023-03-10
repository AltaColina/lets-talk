using System.Security.Claims;

namespace LetsTalk.Services;

public interface ITokenProvider
{
    string GenerateAccessToken(ClaimsIdentity identity, out DateTimeOffset expiresIn);
    string GenerateRefreshToken(ClaimsIdentity identity, out DateTimeOffset expiresIn);
}
