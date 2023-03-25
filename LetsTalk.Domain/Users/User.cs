using IdentityModel;
using System.Security.Claims;

namespace LetsTalk.Users;

public sealed class User : Entity
{
    [SensitiveData]
    public required string Secret { get; set; }
    public required string Email { get; set; }
    public bool IsEmailVerified { get; set; }
    public string? Image { get; set; }
    public DateTimeOffset LastLoginTime { get; set; }
    public bool IsActive { get; set; } = true;
    public HashSet<string> Roles { get; init; } = new();
    public HashSet<string> Rooms { get; init; } = new();
    public HashSet<string> RefreshTokens { get; set; } = new();

    public List<Claim> GetClaims()
    {
        var claims = new List<Claim>(4 + Roles.Count)
        {
            new Claim(JwtClaimTypes.Subject, Id),
            new Claim(JwtClaimTypes.Name, Name),
            new Claim(JwtClaimTypes.Email, Email),
            new Claim(JwtClaimTypes.EmailVerified, IsEmailVerified ? "true" : "false", ClaimValueTypes.Boolean)
        };
        if (!String.IsNullOrWhiteSpace(Image))
            claims.Add(new Claim(JwtClaimTypes.Picture, Image));
        foreach (var role in Roles)
            claims.Add(new Claim(JwtClaimTypes.Role, role));
        return claims;
    }
}
