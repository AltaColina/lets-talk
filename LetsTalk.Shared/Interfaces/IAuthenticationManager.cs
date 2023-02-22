using LetsTalk.Security;
using LetsTalk.Security.Commands;

namespace LetsTalk.Interfaces;

public interface IAuthenticationManager
{
    Task<Authentication> AuthenticateAsync(RegisterCommand request);
    Task<Authentication> AuthenticateAsync(LoginCommand request);
    Task<Authentication> AuthenticateAsync(RefreshCommand request);
}
