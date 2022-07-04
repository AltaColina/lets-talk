using LetsTalk.Models;

namespace LetsTalk.Interfaces;

public interface IAuthenticationManager
{
    Task<AuthenticationResponse> AuthenticateAsync(RegisterRequest request);
    Task<AuthenticationResponse> AuthenticateAsync(LoginRequest request);
    Task<AuthenticationResponse> AuthenticateAsync(RefreshRequest request);
}
