using LetsTalk.Exceptions;
using LetsTalk.Interfaces;
using LetsTalk.Models;
using MediatR;

namespace LetsTalk.Commands;

public sealed class ChatPutRequestHandler : IRequestHandler<ChatPutRequest>
{
    private readonly IChatRepository _chatRepository;

    public ChatPutRequestHandler(IChatRepository chatRepository)
    {
        _chatRepository = chatRepository;
    }

    public async Task<Unit> Handle(ChatPutRequest request, CancellationToken cancellationToken)
    {
        var chat = await _chatRepository.GetAsync(request.Chat.Id);
        if (chat is null)
            throw new NotFoundException($"Chat {request.Chat.Id} does not exist");
        await _chatRepository.UpdateAsync(chat);
        return Unit.Value;
    }
}