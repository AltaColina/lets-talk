using MediatR;

namespace LetsTalk.Dtos.Chats;

public sealed class DeleteChatRequest : IRequest
{
    public string Id { get; init; } = null!;
}