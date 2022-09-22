using LetsTalk.Dtos;

namespace LetsTalk.Models;

public sealed class Authentication
{
    public UserDto User { get; init; } = null!;
    public Token AccessToken { get; init; } = null!;
    public Token RefreshToken { get; init; } = null!;
}