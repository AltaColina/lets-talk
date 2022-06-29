using LetsTalk.Exceptions;
using LetsTalk.Interfaces;
using LetsTalk.Models;
using MediatR;

namespace LetsTalk.Commands;

public sealed class LeaveRequestHandler : IRequestHandler<LeaveRequest>
{
    private readonly IChatRepository _chatRepository;

    public LeaveRequestHandler(IChatRepository chatRepository)
    {
        _chatRepository = chatRepository;
    }

    public async Task<Unit> Handle(LeaveRequest request, CancellationToken cancellationToken)
    {
        var chatRoom = await _chatRepository.GetAsync(request.ChatId);
        if (chatRoom is null)
            throw new NotFoundException($"Chat {request.ChatId} does not exist");
        await chatRoom.LeaveAsync(request.Username);
        return Unit.Value;
    }
}
