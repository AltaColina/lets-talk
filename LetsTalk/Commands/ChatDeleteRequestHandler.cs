using LetsTalk.Exceptions;
using LetsTalk.Interfaces;
using LetsTalk.Models;
using MediatR;

namespace LetsTalk.Commands;

public sealed class ChatDeleteRequestHandler : IRequestHandler<ChatDeleteRequest>
{
    private readonly IChatRepository _chatRepository;

    public ChatDeleteRequestHandler(IChatRepository chatRepository)
    {
        _chatRepository = chatRepository;
    }

    public async Task<Unit> Handle(ChatDeleteRequest request, CancellationToken cancellationToken)
    {
        try
        {
            await _chatRepository.DeleteAsync(request.ChatId);
        }
        catch(Exception ex)
        {
            throw new NotFoundException(ex.Message);
        }

        return Unit.Value;
    }
}