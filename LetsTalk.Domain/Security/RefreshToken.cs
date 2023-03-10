namespace LetsTalk.Security;

public sealed class RefreshToken
{
    public required string Value { get; init; }
    public required DateTimeOffset ExpiresIn { get; init; }
}