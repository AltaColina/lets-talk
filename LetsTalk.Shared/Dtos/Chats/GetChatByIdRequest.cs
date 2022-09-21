using MediatR;

namespace LetsTalk.Dtos.Chats;

public sealed class GetChatByIdRequest : IRequest<ChatDto>
{
    public string Id { get; init; } = null!;
}