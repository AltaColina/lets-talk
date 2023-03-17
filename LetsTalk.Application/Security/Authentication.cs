using LetsTalk.Users;

namespace LetsTalk.Security;

public sealed class Authentication
{
    public required UserDto User { get; init; }

    [SensitiveData]
    public required string AccessToken { get; init; }

    public required DateTimeOffset AccessTokenExpires { get; init; }

    [SensitiveData]
    public required string RefreshToken { get; init; }

    public required DateTimeOffset RefreshTokenExpires { get; init; }

    public required HashSet<string> Permissions { get; init; }
}