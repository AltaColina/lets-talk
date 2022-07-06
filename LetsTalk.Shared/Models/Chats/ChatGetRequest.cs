using MediatR;

namespace LetsTalk.Models.Chats;

public sealed class ChatGetRequest : IRequest<ChatGetResponse>
{
    public string? ChatId { get; init; }
}