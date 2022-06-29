using LetsTalk.Models;

namespace LetsTalk.Interfaces;

public interface IAuthenticationManager
{
    Token GenerateToken(User user);
    Task<Token?> AuthenticateAsync(string username, string password);
    Task<Token?> RefreshAsync(string username, string refreshToken);
}
