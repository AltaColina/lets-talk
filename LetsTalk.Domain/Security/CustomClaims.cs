using System.Security.Claims;

namespace LetsTalk.Security;
public static class CustomClaims
{
    public const string Id = ClaimTypes.Name;
    public const string Username = $"{ClaimTypes.Name}/username";
    public const string ImageUrl = $"{ClaimTypes.Name}/imageurl";
}
