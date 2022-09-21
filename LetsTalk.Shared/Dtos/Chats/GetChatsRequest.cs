using MediatR;

namespace LetsTalk.Dtos.Chats;

public sealed class GetChatsRequest : IRequest<GetChatsResponse>
{
    public string? Id { get; init; }
}