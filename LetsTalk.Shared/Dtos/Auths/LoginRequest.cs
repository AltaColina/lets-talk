using MediatR;
using LetsTalk.Models;

namespace LetsTalk.Dtos.Auths;

public sealed class LoginRequest : IRequest<Authentication>
{
    public string Username { get; init; } = null!;
    public string Password { get; init; } = null!;
}
