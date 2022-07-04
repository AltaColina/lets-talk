using LetsTalk.Exceptions;
using LetsTalk.Interfaces;
using LetsTalk.Models;
using MediatR;

namespace LetsTalk.Commands;

public sealed class PutChatRequestHandler : IRequestHandler<ChatPutRequest>
{
    private readonly IChatRepository _chatRepository;

    public PutChatRequestHandler(IChatRepository chatRepository)
    {
        _chatRepository = chatRepository;
    }

    public async Task<Unit> Handle(ChatPutRequest request, CancellationToken cancellationToken)
    {
        var chatRoom = await _chatRepository.GetAsync(request.Chat.Id);
        if (chatRoom is null)
            throw new NotFoundException("Chat room does not exist");
        chatRoom.Name = request.Chat.Name;
        await _chatRepository.UpdateAsync(chatRoom);
        return Unit.Value;
    }
}