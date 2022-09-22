using LetsTalk.Commands.Auths;
using LetsTalk.Models;

namespace LetsTalk.Interfaces;

public interface IAuthenticationManager
{
    Task<Authentication> AuthenticateAsync(RegisterRequest request);
    Task<Authentication> AuthenticateAsync(LoginRequest request);
    Task<Authentication> AuthenticateAsync(RefreshRequest request);
}
