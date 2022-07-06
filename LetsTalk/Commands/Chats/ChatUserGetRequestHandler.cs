using LetsTalk.Exceptions;
using LetsTalk.Interfaces;
using LetsTalk.Models.Chats;
using MediatR;

namespace LetsTalk.Commands.Chats;

public sealed class ChatUserGetRequestHandler : IRequestHandler<ChatUserGetRequest, ChatUserGetResponse>
{
    private readonly IChatRepository _chatRepository;
    private readonly IUserRepository _userRepository;

    public ChatUserGetRequestHandler(IChatRepository chatRepository, IUserRepository userRepository)
    {
        _chatRepository = chatRepository;
        _userRepository = userRepository;
    }

    public async Task<ChatUserGetResponse> Handle(ChatUserGetRequest request, CancellationToken cancellationToken)
    {
        var chat = _chatRepository.GetAsync(request.ChatId);
        if (chat is null)
            throw new NotFoundException($"Chat {request.ChatId} does not exist");
        var users = (await _userRepository.GetAllAsync()).Where(user => user.Chats.Contains(request.ChatId));
        return new ChatUserGetResponse { Users = new(users) };
    }
}
