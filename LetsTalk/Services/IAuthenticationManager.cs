using LetsTalk.Models;

namespace LetsTalk.Services;

public interface IAuthenticationManager
{
    Token GenerateToken(User user);
    Token? Authenticate(string username, string password);
    Token? Refresh(string username, string refreshToken);
}
