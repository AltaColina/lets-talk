using LetsTalk.Exceptions;
using LetsTalk.Interfaces;
using LetsTalk.Models;
using MediatR;

namespace LetsTalk.Commands;

public sealed class GetChatRequestHandler : IRequestHandler<ChatGetRequest, ChatGetResponse>
{
    private readonly IChatRepository _chatRepository;

    public GetChatRequestHandler(IChatRepository chatRepository)
    {
        _chatRepository = chatRepository;
    }

    public async Task<ChatGetResponse> Handle(ChatGetRequest request, CancellationToken cancellationToken)
    {
        var response = new ChatGetResponse();

        switch (request)
        {
            case ChatGetRequest get when !String.IsNullOrWhiteSpace(get.ChatId):
                var chat = await _chatRepository.GetAsync(get.ChatId);
                if (chat is null)
                    throw new NotFoundException($"Chat {get.ChatId} does not exist");
                response.Chats.Add(chat);
                break;
            case ChatGetRequest get when !String.IsNullOrWhiteSpace(get.ChatName):
                var chats = (await _chatRepository.GetAllAsync()).Where(c => c.Name == request.ChatName);
                if (!chats.Any())
                    throw new NotFoundException($"No chats with name {request.ChatName} exist");
                response.Chats.AddRange(chats);
                break;
            default:
                var chatsAll = await _chatRepository.GetAllAsync();
                if (!chatsAll.Any())
                    throw new NotFoundException($"No chats exist");
                response.Chats.AddRange(chatsAll);
                break;
        }

        return response;
    }
}
