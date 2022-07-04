using LetsTalk.Interfaces;
using LetsTalk.Models;
using MediatR;

namespace LetsTalk.Commands;

public sealed class LogoutRequestHandler : IRequestHandler<LogoutRequest>
{
    private readonly IChatRepository _chatRepository;

    public LogoutRequestHandler(IChatRepository chatRepository)
    {
        _chatRepository = chatRepository;
    }

    public async Task<Unit> Handle(LogoutRequest request, CancellationToken cancellationToken)
    {
        //var chatRooms = await _chatRepository.GetAllAsync();
        //await Task.WhenAll(chatRooms.Select(chatRoom => chatRoom.LeaveAsync(request.Username)));
        return await Unit.Task;
    }
}
