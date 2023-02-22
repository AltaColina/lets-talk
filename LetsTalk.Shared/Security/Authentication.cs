using LetsTalk.Users;

namespace LetsTalk.Security;

public sealed class Authentication
{
    public UserDto User { get; init; } = null!;
    public Token AccessToken { get; init; } = null!;
    public Token RefreshToken { get; init; } = null!;
    public HashSet<string> Permissions { get; init; } = null!;
}