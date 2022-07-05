using MediatR;

namespace LetsTalk.Models;

public sealed class ChatGetRequest : IRequest<ChatGetResponse>
{
    public string? ChatId { get; init; }
}
