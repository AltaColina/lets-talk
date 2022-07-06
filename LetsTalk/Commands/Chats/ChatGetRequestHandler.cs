using LetsTalk.Exceptions;
using LetsTalk.Interfaces;
using LetsTalk.Models;
using LetsTalk.Models.Chats;
using MediatR;

namespace LetsTalk.Commands.Chats;

public sealed class ChatGetRequestHandler : IRequestHandler<ChatGetRequest, ChatGetResponse>
{
    private readonly IChatRepository _chatRepository;

    public ChatGetRequestHandler(IChatRepository chatRepository)
    {
        _chatRepository = chatRepository;
    }

    public async Task<ChatGetResponse> Handle(ChatGetRequest request, CancellationToken cancellationToken)
    {
        if (request.ChatId is null)
            return new ChatGetResponse { Chats = new List<Chat>(await _chatRepository.GetAllAsync()) };

        var chat = await _chatRepository.GetAsync(request.ChatId);
        if (chat is null)
            throw new NotFoundException($"Chat {request.ChatId} does not exist");
        return new ChatGetResponse { Chats = { chat } };
    }
}
