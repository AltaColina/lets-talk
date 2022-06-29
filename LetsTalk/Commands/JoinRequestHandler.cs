using LetsTalk.Exceptions;
using LetsTalk.Interfaces;
using LetsTalk.Models;
using MediatR;

namespace LetsTalk.Commands;

public sealed class JoinRequestHandler : IRequestHandler<JoinRequest>
{
    private readonly IChatRepository _chatRepository;

    public JoinRequestHandler(IChatRepository chatRepository)
    {
        _chatRepository = chatRepository;
    }

    public async Task<Unit> Handle(JoinRequest request, CancellationToken cancellationToken)
    {
        var chatRoom = await _chatRepository.GetAsync(request.ChatId);
        if (chatRoom is null)
            throw new NotFoundException($"Chat {request.ChatId} does not exist");
        await chatRoom.JoinAsync(request.Username, request.ResponseStream!);
        return Unit.Value;
    }
}
