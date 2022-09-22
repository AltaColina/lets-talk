using LetsTalk.Interfaces;
using LetsTalk.Models;

namespace LetsTalk.Dtos;

public sealed class ChatDto : IMapFrom<Chat>
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
}