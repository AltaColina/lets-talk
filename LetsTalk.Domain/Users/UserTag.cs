using LetsTalk.Security;
using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;

namespace LetsTalk.Users;

public sealed class UserTag
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public string? ImageUrl { get; init; }

    public UserTag() { }

    [SetsRequiredMembers]
    public UserTag(ClaimsPrincipal claims)
    {
        Id = claims.FindFirst(CustomClaims.Id)?.Value ?? throw new ArgumentException("Invalid claims", nameof(claims));
        Name = claims.FindFirst(CustomClaims.Username)?.Value ?? throw new ArgumentException("Invalid claims", nameof(claims));
        ImageUrl = claims.FindFirst(CustomClaims.ImageUrl)?.Value;
    }
}