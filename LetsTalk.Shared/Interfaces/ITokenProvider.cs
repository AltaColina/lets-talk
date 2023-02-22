using LetsTalk.Security;
using System.Security.Claims;

namespace LetsTalk.Interfaces;

public interface ITokenProvider
{
    Token GenerateAccessToken(ClaimsIdentity identity);
    Token GenerateRefreshToken(ClaimsIdentity identity);
}
