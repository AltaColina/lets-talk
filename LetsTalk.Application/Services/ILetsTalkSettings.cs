using LetsTalk.Security;
using System.Diagnostics.CodeAnalysis;

namespace LetsTalk.Services;
public interface ILetsTalkSettings
{
    [MemberNotNullWhen(true, nameof(Authentication), nameof(UserId), nameof(AccessToken), nameof(RefreshToken))]
    bool IsAuthenticated { get => Authentication is not null; }

    Authentication? Authentication { get; set; }

    public string? UserId { get => Authentication?.User.Id; }

    public string? AccessToken { get => Authentication?.AccessToken; }

    public string? RefreshToken { get => Authentication?.RefreshToken; }

    Task<string?> ProvideToken();
}