namespace LetsTalk.Security;

public sealed class Token
{
    public required string Id { get; init; }
    public DateTimeOffset ExpiresIn { get; set; }
}