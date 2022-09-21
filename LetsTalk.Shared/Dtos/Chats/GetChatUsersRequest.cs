using MediatR;

namespace LetsTalk.Dtos.Chats;

public sealed class GetChatUsersRequest : IRequest<GetChatUsersResponse>
{
    public string Id { get; init; } = null!;
}
