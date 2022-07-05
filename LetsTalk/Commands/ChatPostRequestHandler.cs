using LetsTalk.Exceptions;
using LetsTalk.Interfaces;
using LetsTalk.Models;
using MediatR;

namespace LetsTalk.Commands;

public sealed class ChatPostRequestHandler : IRequestHandler<ChatPostRequest>
{
    private readonly IChatRepository _chatRepository;

    public ChatPostRequestHandler(IChatRepository chatRepository)
    {
        _chatRepository = chatRepository;
    }
    
    public async Task<Unit> Handle(ChatPostRequest request, CancellationToken cancellationToken)
    {
        var role = await _chatRepository.GetAsync(request.Chat.Id);
        if (role is not null)
            throw new ConflictException($"Role {request.Chat.Id} already exists");
        await _chatRepository.InsertAsync(request.Chat);
        return Unit.Value;
    }
}
