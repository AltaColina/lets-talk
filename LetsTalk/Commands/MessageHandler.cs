using LetsTalk.Exceptions;
using LetsTalk.Interfaces;
using LetsTalk.Models;
using MediatR;

namespace LetsTalk.Commands;

public sealed class MessageHandler : IRequestHandler<Message>
{
    private readonly IChatRepository _chatRepository;

    public MessageHandler(IChatRepository chatRepository)
    {
        _chatRepository = chatRepository;
    }

    public async Task<Unit> Handle(Message request, CancellationToken cancellationToken)
    {
        var chatRoom = await _chatRepository.GetAsync(request.ChatId);
        if (chatRoom is null)
            throw new NotFoundException($"Chat {request.ChatId} does not exist");
        await chatRoom.QueueMessageAsync(request);
        return Unit.Value;
    }
}
