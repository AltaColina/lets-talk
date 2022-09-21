using LetsTalk.Models;
using MediatR;

namespace LetsTalk.Dtos.Auths;

public sealed class RefreshRequest : IRequest<Authentication>
{
    public string Username { get; init; } = null!;
    public string RefreshToken { get; init; } = null!;
}
