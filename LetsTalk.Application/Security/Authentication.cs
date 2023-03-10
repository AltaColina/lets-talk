using LetsTalk.Users;

namespace LetsTalk.Security;

public sealed class Authentication
{
    public required UserDto User { get; init; }
    public required Token AccessToken { get; init; }
    public required Token RefreshToken { get; init; }
    public required HashSet<string> Permissions { get; init; }
}