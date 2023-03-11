using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace LetsTalk.Services;

public interface ITokenProvider
{
    public SecurityToken GenerateAccessToken(ClaimsIdentity identity, out string serializedToken);
    public SecurityToken GenerateRefreshToken(ClaimsIdentity identity, out string serializedToken);
}
