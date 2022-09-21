using LetsTalk.Interfaces;
using LetsTalk.Models;
using MediatR;

namespace LetsTalk.Dtos.Users;

public sealed class UpdateUserRequest : IRequest, IMapTo<User>
{
    public string Id { get; set; } = null!;
    public HashSet<string> Roles { get; set; } = new();
}
