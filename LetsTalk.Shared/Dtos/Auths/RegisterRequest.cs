using LetsTalk.Models;
using MediatR;

namespace LetsTalk.Dtos.Auths;

public sealed class RegisterRequest : IRequest<Authentication>
{
    public string Username { get; init; } = null!;
    public string Password { get; init; } = null!;
    public string? Name { get; init; }
}
