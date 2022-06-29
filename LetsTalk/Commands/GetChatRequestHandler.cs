using LetsTalk.Exceptions;
using LetsTalk.Interfaces;
using LetsTalk.Models;
using MediatR;

namespace LetsTalk.Commands;

public sealed class GetChatRequestHandler : IRequestHandler<GetChatRequest, GetChatResponse>
{
    private readonly IChatRepository _chatRepository;

    public GetChatRequestHandler(IChatRepository chatRepository)
    {
        _chatRepository = chatRepository;
    }

    public async Task<GetChatResponse> Handle(GetChatRequest request, CancellationToken cancellationToken)
    {
        var response = new GetChatResponse();

        switch (request.FilterCase)
        {
            case GetChatRequest.FilterOneofCase.ChatId:
                var chatRoom = await _chatRepository.GetAsync(request.ChatId);
                if (chatRoom is null)
                    throw new NotFoundException($"Chat {request.ChatId} does not exist");
                response.Chats.Add(new ChatInfo { Id = chatRoom.Id, Name = chatRoom.Name });
                break;
            case GetChatRequest.FilterOneofCase.ChatName:
                var chatRooms = (await _chatRepository.GetAllAsync()).Where(c => c.Name == request.ChatName);
                if (!chatRooms.Any())
                    throw new NotFoundException($"No chats with name {request.ChatName} exist");
                response.Chats.AddRange(chatRooms.Select(c => new ChatInfo { Id = c.Id, Name = c.Name }));
                break;
            default:
                var chatRooms1 = (await _chatRepository.GetAllAsync());
                if (!chatRooms1.Any())
                    throw new NotFoundException($"No chats exist");
                response.Chats.AddRange(chatRooms1.Select(c => new ChatInfo { Id = c.Id, Name = c.Name }));
                break;
        }

        return response;
    }
}
