using LetsTalk.Exceptions;
using LetsTalk.Interfaces;
using LetsTalk.Models.Chats;
using MediatR;

namespace LetsTalk.Commands.Chats;

public sealed class ChatUserPutRequestHandler : IRequestHandler<ChatUserPutRequest>
{
    private readonly IChatRepository _chatRepository;
    private readonly IUserRepository _userRepository;

    public ChatUserPutRequestHandler(IChatRepository chatRepository, IUserRepository userRepository)
    {
        _chatRepository = chatRepository;
        _userRepository = userRepository;
    }

    public async Task<Unit> Handle(ChatUserPutRequest request, CancellationToken cancellationToken)
    {
        var chat = await _chatRepository.GetAsync(request.ChatId);
        if (chat is null)
            throw new NotFoundException($"Chat {request.ChatId} does not exist");
        var user = await _userRepository.GetAsync(request.UserId);
        if (user is null)
            throw new NotFoundException($"User {request.UserId} does not exist");

        user.Chats.Add(chat.Id);
        await _chatRepository.UpdateAsync(chat);
        return Unit.Value;
    }
}