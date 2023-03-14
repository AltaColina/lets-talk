using LetsTalk.Security;
using System.Security.Claims;

namespace LetsTalk;

public static class ClaimsPrincipalExtensions
{
    public static string ReadClaim(this ClaimsPrincipal claims, string type)
    {
        if (claims is null)
            throw new ArgumentNullException(nameof(claims));

        return claims.FindFirst(type)?.Value ?? throw new ArgumentException($"Invalid claims: missing '{type}'", nameof(claims));
    }

    public static string ReadUserIdClaim(this ClaimsPrincipal claims)
    {
        return claims.ReadClaim(CustomClaims.Id);
    }

    public static string ReadUserNameClaim(this ClaimsPrincipal claims)
    {
        return claims.ReadClaim(CustomClaims.Username);
    }

    public static string ReadUserImageUrlClaim(this ClaimsPrincipal claims)
    {
        return claims.ReadClaim(CustomClaims.ImageUrl);
    }

    public static (string UserId, string UserName, string? UserImageUrl) ReadUserClaims(this ClaimsPrincipal claims)
    {
        return (claims.ReadUserIdClaim(), claims.ReadUserNameClaim(), claims.ReadUserImageUrlClaim());
    }
}